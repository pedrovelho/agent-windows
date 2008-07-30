// hungapp.cpp
//
// Application hangup detection.
//
// $Id: $
//

#include "stdafx.h"
#include "hungapp.h"

//---------------------------------------------------------------------------
// IsAppHung_SMTO
//
//  Determines whether the application is hung using SendMessageTimeout
//  function.
//
//  Parameters:
//	  hWnd   - window handle
//	  pbHung - pointer to a boolean variable that receives TRUE, if the
//			   application is hung
//
//  Returns:
//	  TRUE, if successful, FALSE - otherwise.
//
BOOL
WINAPI
IsAppHung_SMTO(
	IN HWND hWnd,
	OUT PBOOL pbHung
	)
{
	_ASSERTE(pbHung != NULL);

	*pbHung = FALSE;

	if (!IsWindow(hWnd))
		return SetLastError(ERROR_INVALID_PARAMETER), FALSE;

	DWORD_PTR dwResult;
	if (!SendMessageTimeout(hWnd, WM_NULL, 0, 0, 
							SMTO_ABORTIFHUNG|SMTO_BLOCK, 500,
							&dwResult))
		*pbHung = TRUE;

	return TRUE;
}

//---------------------------------------------------------------------------
// IsAppHung_Undoc
//
//  Determines whether the application is hung using undocumented
//	IsHungAppWindow on Windows NT and IsHungThread on Windows 9x.
//
//  Parameters:
//	  hWnd	 - window handle
//	  pbHung - pointer to a boolean variable that receives TRUE, if the
//			   application is hung
//  
//  Returns:
//	  TRUE, if successful, FALSE - otherwise.
//
BOOL
WINAPI
IsAppHung_Undoc(
	IN HWND hWnd,
	OUT PBOOL pbHung
	)
{
	if (!IsWindow(hWnd))
		return SetLastError(ERROR_INVALID_PARAMETER), FALSE;
	
	OSVERSIONINFO osvi;
	osvi.dwOSVersionInfoSize = sizeof(osvi);

	// determine operating system version
	GetVersionEx(&osvi);

	// obtain USER32.DLL instance handle
	HINSTANCE hUser = GetModuleHandle(_T("user32.dll"));
	_ASSERTE(hUser != NULL);

	if (osvi.dwPlatformId == VER_PLATFORM_WIN32_NT)
	{
		BOOL (WINAPI * _IsHungAppWindow)(HWND);

		// find IsHungAppWindow entry point
		*(FARPROC *)&_IsHungAppWindow =
			GetProcAddress(hUser, "IsHungAppWindow");
		if (_IsHungAppWindow == NULL)
			return SetLastError(ERROR_PROC_NOT_FOUND), FALSE;

		// call IsHungAppWindow
		*pbHung = _IsHungAppWindow(hWnd);
	}
	else
	{
		DWORD dwThreadId = GetWindowThreadProcessId(hWnd, NULL);

		BOOL (WINAPI * _IsHungThread)(DWORD);

		// find IsHungThread entry point
		*(FARPROC *)&_IsHungThread =
			GetProcAddress(hUser, "IsHungThread");
		if (_IsHungThread == NULL)
			return SetLastError(ERROR_PROC_NOT_FOUND), FALSE;

		// call IsHungThread
		*pbHung = _IsHungThread(dwThreadId);
	}

	return TRUE;
}
