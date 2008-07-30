// killapp.cpp
//
// Termination of applications.
//
// $Id: $
//

#include "stdafx.h"
#include "killapp.h"

//---------------------------------------------------------------------------
// EnumProcessWOWProc
//
//  This is a callback function for VDMEnumProcessWOW. It is use to check
//  if the specified process is a VDM process.
//
//  Parameters:
//	   dwProcessId  - VDM process identifier
//	   dwAttributes - VDM attributes
//	   lParam       - contains a pointer to a variable that holds the process
//					  identifier
//
//  Returns:
//	  FALSE - to continue enumeration, TRUE - to stop enumeration.
//
static
BOOL
CALLBACK
EnumProcessWOWProc(
	IN DWORD dwProcessId,
	IN DWORD dwAttributes,
	IN LPARAM lParam
	)
{
	dwAttributes;	// make compiler happy

	_ASSERTE(lParam != 0);

	if (dwProcessId == *(DWORD *)lParam)
	{
		// zero out process identifier to indicate we have found a match
		*(DWORD *)lParam = 0;
		return TRUE;
	}

	return FALSE;
}

//---------------------------------------------------------------------------
// IsWOWProcess
//
//  Determines whether the specified process is a WOW VDM process.
//
//  Parameters:
//    hVdmDbg     - VDMDBG.DLL instance handle
//	  dwProcessId - process identifier
//
//  Returns:
//	  TRUE, if the process is a WOW VDM process, FALSE - otherwise, or if
//	  an error occurs.
//
static
BOOL
WINAPI
IsWOWProcess(
	IN HINSTANCE hVdmDbg,
	IN DWORD dwProcessId
	)
{
	_ASSERTE(hVdmDbg != NULL);
	_ASSERTE(dwProcessId != 0);

	int (WINAPI * _VDMEnumProcessWOW)(PROCESSENUMPROC, LPARAM);

	*(FARPROC *)&_VDMEnumProcessWOW = 
		GetProcAddress(hVdmDbg, "VDMEnumProcessWOW");
	_ASSERTE(_VDMEnumProcessWOW != NULL);

	_VDMEnumProcessWOW(EnumProcessWOWProc, (LPARAM)&dwProcessId);
	return dwProcessId == 0;
}

//---------------------------------------------------------------------------
// KillAppEnumWindows
//
//  This is a callback function for EnumWindows. It sends the WM_CLOSE
//  message for each top-level window, belonging to the specified process.
//
//  Parameters:
//    hWnd   - window handle
//	  lParam - contains the process identifier
//
//  Returns:
//    TRUE - to continue enumeration, FALSE - to stop it.
//
static
BOOL
CALLBACK
KillAppEnumWindows(
	IN HWND hWnd,
	IN LPARAM lParam
	)
{
	_ASSERTE(lParam != 0);

	DWORD dwProcessId;
	GetWindowThreadProcessId(hWnd, &dwProcessId);

	if (IsWindowVisible(hWnd) &&
		dwProcessId == (DWORD)lParam)
		PostMessage(hWnd, WM_CLOSE, 0, 0);

	return TRUE;
}

//---------------------------------------------------------------------------
// KillAppEnumWindows16
//
//  This is a callback function for EnumWindows. It sends the WM_CLOSE
//	message for each top-level window, belonging to the specified thread.
//  This is used to stop 16-bit applications because a 16-bit application
//	is a thread within a WOW VDM process.
//
//  Parameters:
//	  hWnd   - window handle
//	  lParam - contains the thread identifier
//
//  Returns:
//	  TRUE - to continue enumeration, FALSE - to stop it.
//
static
BOOL
CALLBACK
KillAppEnumWindows16(
	IN HWND hWnd,
	IN LPARAM lParam
	)
{
	_ASSERTE(lParam != 0);

	if (IsWindowVisible(hWnd) &&
		GetWindowThreadProcessId(hWnd, NULL) == (DWORD)lParam)
		PostMessage(hWnd, WM_CLOSE, 0, 0);

	return TRUE;
}

typedef struct _WOWTASKENUM {
	DWORD	dwThreadId;
	WORD	wTaskId;
} WOWTASKENUM, * PWOWTASKENUM;

//---------------------------------------------------------------------------
// EnumTaskWOWProc
//
//  This is a callback function for VDMEnumTaskWOW. It is used to detect
//	whether a task is still alive.
//
//  Parameters:
//	  dwThreadId - task thread identifier
//	  hMod16     - EXE-file module handle
//	  hTask16    - task identifier
//	  lParam     - contains a pointer to WOWTASKENUM structure
//
//  Returns:
//	  FALSE - to continue enumeration, TRUE - to stop enumeration.
//
static
BOOL
CALLBACK
EnumTaskWOWProc(
	IN DWORD dwThreadId,
	IN WORD hMod16,
	IN WORD hTask16,
	IN LPARAM lParam
	)
{
	hMod16; // make compiler happy

	PWOWTASKENUM pTaskEnum = (PWOWTASKENUM)lParam;
	_ASSERTE(pTaskEnum != NULL);

	if (dwThreadId == pTaskEnum->dwThreadId)
	{
		pTaskEnum->wTaskId = hTask16;
		return TRUE;
	}

	return FALSE;
}
   
//---------------------------------------------------------------------------
// IsWOWTask
//
//  Checks whether a WOW task is still alive and returns the task identifier
//	for the task.
//
//  Parameters:
//	  hVdmDbg     - VDMDBG.DLL instance handle
//	  dwProcessId - WOW VDM process identifier
//    dwThreadId  - task thread identifier
//
//  Returns:
//	  if the task is still alive, the return value is the task identifier,
//	  if the task has gone, the return value is zero
//
static
WORD
WINAPI
IsWOWTask(
	IN HINSTANCE hVdmDbg,
	IN DWORD dwProcessId,
	IN DWORD dwThreadId
	)
{
	_ASSERTE(hVdmDbg != NULL);
	_ASSERTE(dwProcessId != 0);
	_ASSERTE(dwThreadId != 0);

	int (WINAPI * _VDMEnumTaskWOW)(DWORD, TASKENUMPROC, LPARAM);

	*(FARPROC *)&_VDMEnumTaskWOW = 
		GetProcAddress(hVdmDbg, "VDMEnumTaskWOW");

	WOWTASKENUM wte;
	wte.dwThreadId = dwThreadId;
	wte.wTaskId = 0;

	_VDMEnumTaskWOW(dwProcessId, EnumTaskWOWProc, (LPARAM)&wte);
	return wte.wTaskId;
}

//---------------------------------------------------------------------------
// TerminateWOWTask
//
//  Terminates a 16-bit task.
//
//  Parameters:
//	  hVdmDbg     - VDMDBG.DLL instance handle
//	  dwProcessId - WOW VDM process identifier
//	  wTaskId     - task identifier
//
//  Returns:
//	  TRUE, if successful, FALSE - otherwise.
//
static
BOOL
WINAPI
TerminateWOWTask(
	IN HINSTANCE hVdmDbg,
	IN DWORD dwProcessId,
	IN WORD wTaskId
	)
{
	_ASSERTE(hVdmDbg != NULL);
	_ASSERTE(dwProcessId != 0);
	_ASSERTE(wTaskId != 0);

	BOOL (WINAPI * _VDMTerminateTaskWOW)(DWORD, WORD);

	*(FARPROC *)&_VDMTerminateTaskWOW =
		GetProcAddress(hVdmDbg, "VDMTerminateTaskWOW");

	return _VDMTerminateTaskWOW(dwProcessId, wTaskId);
}

//---------------------------------------------------------------------------
// KillApplication
//
//  Terminates the specified application.
//
//  Parameters:
//	  hWnd            - window handle
//	  pfnWaitCallback - pointer to a caller-supplied function which is
//					    called periodically to allow the caller process
//						message
//	  lParam		  - parameter to pass to the caller-supplied function
//
//  Returns:
//	  TRUE, if successful, FALSE - otherwise.
//
BOOL
WINAPI
KillApplication(
	IN HWND hWnd,
	IN PFNWAITCALLBACK pfnWaitCallback,
	IN LPARAM lParam
	)
{
	_ASSERTE(pfnWaitCallback != NULL);

	if (!IsWindow(hWnd))
		return SetLastError(ERROR_INVALID_PARAMETER), FALSE;

	DWORD dwProcessId;
	DWORD dwThreadId = GetWindowThreadProcessId(hWnd, &dwProcessId);

	OSVERSIONINFO osvi;
	osvi.dwOSVersionInfoSize = sizeof(osvi);

	// determine operating system version
	GetVersionEx(&osvi);	

	// first of all, determine whether this is a 16-bit application
	// on Windows NT
	BOOL b16bit = FALSE;
	HINSTANCE hVdmDbg = NULL;

	if (osvi.dwPlatformId == VER_PLATFORM_WIN32_NT)
	{
		// load VDMDBG.DLL
		hVdmDbg = LoadLibrary(_T("vdmdbg.dll"));
		if (hVdmDbg == NULL)
			return FALSE;

		b16bit = IsWOWProcess(hVdmDbg, dwProcessId);
	}

	if (!b16bit)
	{
		// open process handle
		HANDLE hProcess = OpenProcess(SYNCHRONIZE|PROCESS_TERMINATE,
									  FALSE, dwProcessId);
		if (hProcess == NULL)
		{
			DWORD dwError = GetLastError();

			if (hVdmDbg != NULL)
				FreeLibrary(hVdmDbg);

			return SetLastError(dwError), FALSE;
		}

		// send WM_CLOSE message for each top-level window in the
		// process
		EnumWindows(KillAppEnumWindows, dwProcessId);

		int nRet = KILLAPP_WAIT;
		while (WaitForSingleObject(hProcess, 100) == WAIT_TIMEOUT)
		{
			nRet = pfnWaitCallback(lParam);
			if (nRet != KILLAPP_WAIT)
				break;
		}

		if (nRet == KILLAPP_TERMINATE)
			TerminateProcess(hProcess, (UINT)-1);

		CloseHandle(hProcess);
	}
	else
	{
		// send WM_CLOSE message for each top-level window in the
		// thread
		EnumWindows(KillAppEnumWindows16, dwThreadId);

		// wait until this 16-bit task ceases to exist
		int nRet = KILLAPP_WAIT;
		WORD wTaskId;
		while ((wTaskId = IsWOWTask(hVdmDbg, dwProcessId, dwThreadId)) != 0)
		{
			Sleep(100);

			nRet = pfnWaitCallback(lParam);
			if (nRet != KILLAPP_WAIT)
				break;
		}

		if (nRet == KILLAPP_TERMINATE)
			TerminateWOWTask(hVdmDbg, dwProcessId, wTaskId);
	}

	if (hVdmDbg != NULL)
		FreeLibrary(hVdmDbg);

	return TRUE;
}
