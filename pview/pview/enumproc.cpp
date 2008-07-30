// enumproc.cpp
//
// Enumeration of processes.
//
// $Id: $
//

#include "stdafx.h"
#include "enumproc.h"

#include <tlhelp32.h>
#include <pdh.h>
#include <pdhmsg.h>

#include <comdef.h>
#include <SshWbemHelpers.h>

// NOTE: there are some OS-specific APIs are called throughout this module.
//		 we cannot link to them implicitly, because it automatically renders
//		 the program unusable on other systems there these APIs don't exist.
//		 Therefore, we have to dynamically link with all OS-specific APIs to
//		 ensure that the program is runnable on all systems.

//---------------------------------------------------------------------------
// EnumProcesses_PsApi
//
//  Enumerates all processes in the system using PSAPI.
//
//  Parameters:
//	  pszMachineName - must be NULL
//	  pfnEnumProc    - address of the function to call for each process
//	  lParam	     - additional parameter to pass to the function
//
//  Returns:
//	  TRUE, if successful, FALSE - otherwise.
//
BOOL
WINAPI
EnumProcesses_PsApi(
	IN LPCTSTR pszMachineName,
	IN PFNENUMPROC pfnEnumProc,
	IN LPARAM lParam
	)
{
	_UNUSED(pszMachineName);

	_ASSERTE(pfnEnumProc != NULL);
	_ASSERTE(pszMachineName == NULL);

	HINSTANCE hPsApi;
	BOOL (WINAPI * _EnumProcesses)(DWORD *, DWORD, DWORD *);
	BOOL (WINAPI * _EnumProcessModules)(HANDLE, HMODULE *, DWORD, DWORD *);
	BOOL (WINAPI * _GetModuleFileNameEx)(HANDLE, HMODULE, LPTSTR, DWORD);
	
	// load PSAPI.DLL
	hPsApi = LoadLibrary(_T("psapi.dll"));
	if (hPsApi == NULL)
		return FALSE;

	// find necessary entry points in PSAPI.DLL
	*(FARPROC *)&_EnumProcesses = 
		GetProcAddress(hPsApi, "EnumProcesses");
	*(FARPROC *)&_EnumProcessModules =
		GetProcAddress(hPsApi, "EnumProcessModules");
#ifdef UNICODE
	*(FARPROC *)&_GetModuleFileNameEx =
		GetProcAddress(hPsApi, "GetModuleFileNameExW");
#else
	*(FARPROC *)&_GetModuleFileNameEx =
		GetProcAddress(hPsApi, "GetModuleFileNameExA");
#endif

	if (_EnumProcesses == NULL ||
		_EnumProcessModules == NULL ||
		_GetModuleFileNameEx == NULL)
	{
		FreeLibrary(hPsApi);
		return SetLastError(ERROR_PROC_NOT_FOUND), FALSE;
	}

	// obtain a handle to the default process heap
	HANDLE hHeap = GetProcessHeap();

	DWORD dwError;
	DWORD cbReturned;
	DWORD cbAlloc = 128;
	DWORD * pdwIds = NULL;

	OSVERSIONINFO osvi;
	osvi.dwOSVersionInfoSize = sizeof(osvi);
	GetVersionEx(&osvi);

	DWORD dwSystemId = 8;
    if (osvi.dwMajorVersion >= 5)
        dwSystemId = (osvi.dwMinorVersion == 0) ? 2 : 4;

	do
	{
		cbAlloc *= 2;

		if (pdwIds != NULL)
			HeapFree(hHeap, 0, pdwIds);

		// allocate memory for the array of process identifiers
		pdwIds = (DWORD *)HeapAlloc(hHeap, 0, cbAlloc);
		if (pdwIds == NULL)
		{
			FreeLibrary(hPsApi);
			return SetLastError(ERROR_NOT_ENOUGH_MEMORY), FALSE;
		}

		// retrieve process identifiers
		if (!_EnumProcesses(pdwIds, cbAlloc, &cbReturned))
		{
			dwError = GetLastError();

			HeapFree(hHeap, 0, pdwIds);
			FreeLibrary(hPsApi);
			return SetLastError(dwError), FALSE;
		}
	}
	while (cbReturned == cbAlloc);

	// now loop through the process identifiers and call the callback
	// function for each process identifier obtained
	for (DWORD i = 0; i < cbReturned / sizeof(DWORD); i++)
	{
		BOOL bContinue;
		DWORD dwProcessId = pdwIds[i];

		// handle two special cases: Idle process (0) and System process
		if (dwProcessId == 0)
		{
			bContinue = pfnEnumProc(0, _T("Idle"), lParam);
		}
		else if (dwProcessId == dwSystemId)
		{
			bContinue = pfnEnumProc(dwSystemId, _T("System"), lParam);
		}
		else
		{
			HANDLE hProcess;
			HMODULE hExeModule;
			DWORD cbNeeded;
			TCHAR szModulePath[MAX_PATH];
			LPTSTR pszProcessName = NULL;

			// open the process handle
			hProcess = OpenProcess(PROCESS_QUERY_INFORMATION|PROCESS_VM_READ,
								   FALSE, dwProcessId);
			if (hProcess != NULL)
			{
				if (_EnumProcessModules(hProcess, &hExeModule,
										sizeof(HMODULE), &cbNeeded))
				{
					if (_GetModuleFileNameEx(hProcess, hExeModule,
											 szModulePath, MAX_PATH))
					{
						pszProcessName = _tcsrchr(szModulePath, _T('\\'));
						if (pszProcessName == NULL)
							pszProcessName = szModulePath;
						else
							pszProcessName++;
					}
				}
			}

			CloseHandle(hProcess);

			// enumerate this process
			bContinue = pfnEnumProc(dwProcessId, pszProcessName, lParam);
		}

		if (!bContinue)
			break;
	}

	HeapFree(hHeap, 0, pdwIds);
	FreeLibrary(hPsApi);

	return TRUE;
}

//---------------------------------------------------------------------------
// EnumProcesses_ToolHelp
//
//  Enumerates all processes in the system using ToolHelp library.
//
//  Parameters:
//	  pszMachineName - must be NULL
//	  pfnEnumProc    - address of the function to call for each process
//	  lParam	     - additional parameter to pass to the function
//
//  Returns:
//	  TRUE, if successful, FALSE - otherwise.
//
BOOL
WINAPI
EnumProcesses_ToolHelp(
	IN LPCTSTR pszMachineName,
	IN PFNENUMPROC pfnEnumProc,
	IN LPARAM lParam
	)
{
	_UNUSED(pszMachineName);

	_ASSERTE(pfnEnumProc != NULL);
	_ASSERTE(pszMachineName == NULL);

	HINSTANCE hKernel;
	HANDLE (WINAPI * _CreateToolhelp32Snapshot)(DWORD, DWORD);
	BOOL (WINAPI * _Process32First)(HANDLE, PROCESSENTRY32 *);
	BOOL (WINAPI * _Process32Next)(HANDLE, PROCESSENTRY32 *);

	// get handle to KERNEL32.DLL
	hKernel = GetModuleHandle(_T("kernel32.dll"));
	_ASSERTE(hKernel != NULL);

	// locate all necessary functions in KERNEL32.DLL
	*(FARPROC *)&_CreateToolhelp32Snapshot =
		GetProcAddress(hKernel, "CreateToolhelp32Snapshot");
#ifdef _UNICODE
	*(FARPROC *)&_Process32First =
		GetProcAddress(hKernel, "Process32FirstW");
	*(FARPROC *)&_Process32Next =
		GetProcAddress(hKernel, "Process32NextW");
#else
	*(FARPROC *)&_Process32First =
		GetProcAddress(hKernel, "Process32First");
	*(FARPROC *)&_Process32Next =
		GetProcAddress(hKernel, "Process32Next");
#endif

	if (_CreateToolhelp32Snapshot == NULL ||
		_Process32First == NULL ||
		_Process32Next == NULL)
		return SetLastError(ERROR_PROC_NOT_FOUND), FALSE;

	HANDLE hSnapshot;
	PROCESSENTRY32 Entry;

	// create a snapshot
	hSnapshot = _CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS, 0);
	if (hSnapshot == INVALID_HANDLE_VALUE)
		return FALSE;

	// retrieve information about the first process
	Entry.dwSize = sizeof(Entry);
	if (!_Process32First(hSnapshot, &Entry))
		return FALSE;

	// walk through all processes
	do
	{
		LPTSTR pszProcessName = NULL;

		if (Entry.dwSize > offsetof(PROCESSENTRY32, szExeFile))
		{
			pszProcessName = _tcsrchr(Entry.szExeFile, _T('\\'));
			if (pszProcessName == NULL)
				pszProcessName = Entry.szExeFile;
		}

		if (!pfnEnumProc(Entry.th32ProcessID, pszProcessName, lParam))
			break;

		Entry.dwSize = sizeof(Entry);
	}
	while (_Process32Next(hSnapshot, &Entry));

	CloseHandle(hSnapshot);
	return TRUE;
}

//
// Some definitions from NTDDK and other sources
//

typedef LONG	NTSTATUS;
typedef LONG	KPRIORITY;

#define NT_SUCCESS(Status) ((NTSTATUS)(Status) >= 0)

#define STATUS_INFO_LENGTH_MISMATCH      ((NTSTATUS)0xC0000004L)

#define SystemProcessesAndThreadsInformation	5

typedef struct _CLIENT_ID {
    DWORD	    UniqueProcess;
    DWORD	    UniqueThread;
} CLIENT_ID;

typedef struct _UNICODE_STRING {
    USHORT	    Length;
    USHORT	    MaximumLength;
    PWSTR	    Buffer;
} UNICODE_STRING;

typedef struct _VM_COUNTERS {
    SIZE_T	    PeakVirtualSize;
    SIZE_T	    VirtualSize;
    ULONG	    PageFaultCount;
    SIZE_T	    PeakWorkingSetSize;
    SIZE_T	    WorkingSetSize;
    SIZE_T	    QuotaPeakPagedPoolUsage;
    SIZE_T	    QuotaPagedPoolUsage;
    SIZE_T	    QuotaPeakNonPagedPoolUsage;
    SIZE_T	    QuotaNonPagedPoolUsage;
    SIZE_T	    PagefileUsage;
    SIZE_T	    PeakPagefileUsage;
} VM_COUNTERS;

typedef struct _SYSTEM_THREADS {
    LARGE_INTEGER   KernelTime;
    LARGE_INTEGER   UserTime;
    LARGE_INTEGER   CreateTime;
    ULONG			WaitTime;
    PVOID			StartAddress;
    CLIENT_ID	    ClientId;
    KPRIORITY	    Priority;
    KPRIORITY	    BasePriority;
    ULONG			ContextSwitchCount;
    LONG			State;
    LONG			WaitReason;
} SYSTEM_THREADS, * PSYSTEM_THREADS;

// Note that the size of the SYSTEM_PROCESSES structure is different on
// NT 4 and Win2K, but we don't care about it, since we don't access neither
// IoCounters member nor Threads array

typedef struct _SYSTEM_PROCESSES {
    ULONG			NextEntryDelta;
    ULONG			ThreadCount;
    ULONG			Reserved1[6];
    LARGE_INTEGER   CreateTime;
    LARGE_INTEGER   UserTime;
    LARGE_INTEGER   KernelTime;
    UNICODE_STRING  ProcessName;
    KPRIORITY	    BasePriority;
    ULONG			ProcessId;
    ULONG			InheritedFromProcessId;
    ULONG			HandleCount;
    ULONG			Reserved2[2];
    VM_COUNTERS	    VmCounters;
#if _WIN32_WINNT >= 0x500
    IO_COUNTERS	    IoCounters;
#endif
    SYSTEM_THREADS  Threads[1];
} SYSTEM_PROCESSES, * PSYSTEM_PROCESSES;

//---------------------------------------------------------------------------
// EnumProcesses_NtApi
//
//  Enumerates all processes in the system using NtQuerySystemInformation.
//  
//  Parameters:
//	  pszMachineName - must be NULL
//	  pfnEnumProc    - address of the function to call for each process
//	  lParam	     - additional parameter to pass to the function
//
//  Returns:
//	  TRUE, if successful, FALSE - otherwise.
//
BOOL
WINAPI
EnumProcesses_NtApi(
	IN LPCTSTR pszMachineName,
	IN PFNENUMPROC pfnEnumProc,
	IN LPARAM lParam
	)
{
	_UNUSED(pszMachineName);

	_ASSERTE(pfnEnumProc != NULL);
	_ASSERTE(pszMachineName == NULL);

	HINSTANCE hNtDll;
	NTSTATUS (WINAPI * _ZwQuerySystemInformation)(UINT, PVOID, ULONG, PULONG);

	// get handle to NTDLL.DLL
	hNtDll = GetModuleHandle(_T("ntdll.dll"));
	_ASSERTE(hNtDll != NULL);

	// find the address of ZwQuerySystemInformation
	*(FARPROC *)&_ZwQuerySystemInformation =
		GetProcAddress(hNtDll, "ZwQuerySystemInformation");
	if (_ZwQuerySystemInformation == NULL)
		return SetLastError(ERROR_PROC_NOT_FOUND), FALSE;

	// obtain a handle to the default process heap
	HANDLE hHeap = GetProcessHeap();
	
	NTSTATUS Status;
    ULONG cbBuffer = 0x8000;
    PVOID pBuffer = NULL;

    // it is difficult to say a priory which size of the buffer 
    // will be enough to retrieve all information, so we start
    // with 32K buffer and increase its size until we get the
    // information successfully
    do
    {
		pBuffer = HeapAlloc(hHeap, 0, cbBuffer);
		if (pBuffer == NULL)
			return SetLastError(ERROR_NOT_ENOUGH_MEMORY), FALSE;

		Status = _ZwQuerySystemInformation(
					SystemProcessesAndThreadsInformation,
					pBuffer, cbBuffer, NULL);

		if (Status == STATUS_INFO_LENGTH_MISMATCH)
		{
			HeapFree(hHeap, 0, pBuffer);
			cbBuffer *= 2;
		}
		else if (!NT_SUCCESS(Status))
		{
			HeapFree(hHeap, 0, pBuffer);
			return SetLastError(Status), FALSE;
		}
    }
    while (Status == STATUS_INFO_LENGTH_MISMATCH);

    PSYSTEM_PROCESSES pProcesses = (PSYSTEM_PROCESSES)pBuffer;

    for (;;)
    {
		PCWSTR pszProcessName = pProcesses->ProcessName.Buffer;
		if (pszProcessName == NULL)
			pszProcessName = L"Idle";

#ifdef _UNICODE

		if (!pfnEnumProc(pProcesses->ProcessId, pszProcessName, lParam))
			break;

#else

		CHAR szProcessName[MAX_PATH];
		WideCharToMultiByte(CP_ACP, 0, pszProcessName, -1,
							szProcessName, MAX_PATH, NULL, NULL);

		if (!pfnEnumProc(pProcesses->ProcessId, szProcessName, lParam))
			break;

#endif
		if (pProcesses->NextEntryDelta == 0)
			break;

		// find the address of the next process structure
		pProcesses = (PSYSTEM_PROCESSES)(((LPBYTE)pProcesses)
						+ pProcesses->NextEntryDelta);
	}

	HeapFree(hHeap, 0, pBuffer);
	return TRUE;
}

//---------------------------------------------------------------------------
// PerfFormatCounterPath
//
//  Format the path to the Process\ID Process counter on the specified
//  computer.
//
//  Parameters:
//	  hPdh           - PDH.DLL instance handle
//	  pszMachineName - machine name (may be NULL)
//	  pszPath		 - pointer to a buffer that receives the formatted path
//	  cchPath		 - size of the buffer in characters
//
//  Returns:
//    Win32 or PDH error code.
//
static
PDH_STATUS
WINAPI
PerfFormatCounterPath(
	IN HINSTANCE hPdh,
	IN LPCTSTR pszMachineName,
	IN LPWSTR pszPath,
	IN ULONG cchPath
	)
{
	_ASSERTE(hPdh != NULL);
	_ASSERTE(_CrtIsValidPointer(pszPath, cchPath * sizeof(WCHAR), 1));

	PDH_STATUS (WINAPI * _PdhMakeCounterPath)(
		PDH_COUNTER_PATH_ELEMENTS_W *, LPWSTR, LPDWORD, DWORD);
    PDH_STATUS (WINAPI * _PdhLookupPerfNameByIndex)(
		LPCWSTR, DWORD, LPWSTR, LPDWORD);

	*(FARPROC *)&_PdhMakeCounterPath =
		GetProcAddress(hPdh, "PdhMakeCounterPathW");
	*(FARPROC *)&_PdhLookupPerfNameByIndex =
		GetProcAddress(hPdh, "PdhLookupPerfNameByIndexW");

	if (_PdhMakeCounterPath == NULL ||
		_PdhLookupPerfNameByIndex == NULL)
		return ERROR_PROC_NOT_FOUND;

	PDH_STATUS Status;
	DWORD cbName;
	WCHAR szObjectName[80];
	WCHAR szCounterName[80];

	LPWSTR pszMachineW = NULL;

#ifndef _UNICODE
	WCHAR szMachineName[256];
	if (pszMachineName != NULL)
	{
		MultiByteToWideChar(CP_ACP, 0, pszMachineName, -1, szMachineName, 256);
		pszMachineW = szMachineName;
	}
#else
	pszMachineW = (LPWSTR)pszMachineName;
#endif

	// locate Process object name
	cbName = sizeof(szObjectName);
	Status = _PdhLookupPerfNameByIndex(pszMachineW, 230, 
									   szObjectName, &cbName);
	if (Status != ERROR_SUCCESS)
		return Status == PDH_ACCESS_DENIED ? ERROR_ACCESS_DENIED : Status;

	// locate ID Process counter name
	cbName = sizeof(szCounterName);
    Status = _PdhLookupPerfNameByIndex(pszMachineW, 784,
									   szCounterName, &cbName);
	if (Status != ERROR_SUCCESS)
		return Status == PDH_ACCESS_DENIED ? ERROR_ACCESS_DENIED : Status;

	PDH_COUNTER_PATH_ELEMENTS_W pcpe;
	pcpe.szMachineName = pszMachineW;
	pcpe.szObjectName = szObjectName;
	pcpe.szInstanceName = L"*";
	pcpe.szParentInstance = NULL;
	pcpe.dwInstanceIndex = 0;
	pcpe.szCounterName = szCounterName;

	// prepare counter path
	return _PdhMakeCounterPath(&pcpe, pszPath, &cchPath, 0);
}

//---------------------------------------------------------------------------
// Q&A unplugged: PDH authentication
//
// PerfAuthenticate
//
//  Establishes an authenticated connection with the specified machine.
//  This function needs to be called only once during session and affects
//  all programs in this user session.
//
//  Parameters:
//	  pszMachineName - machine name
//	  pszUserName    - user name
//	  pszPassword    - password
//
//  Returns:
//	  Win32 error code.
//
DWORD
WINAPI
PerfAuthenticate(
	IN LPCTSTR pszMachineName,
	IN LPCTSTR pszUserName,
	IN LPCTSTR pszPassword
	)
{
	_ASSERTE(pszMachineName != NULL);
	_ASSERTE(pszUserName != NULL);
	_ASSERTE(pszPassword != NULL);

	TCHAR szRemoteName[MAX_PATH];
	wsprintf(szRemoteName, _T("%s\\IPC$"), (LPCTSTR)pszMachineName);

	NETRESOURCE NetRes;
	NetRes.dwType = RESOURCETYPE_ANY;
	NetRes.lpLocalName = NULL;
	NetRes.lpRemoteName = szRemoteName;
	NetRes.lpProvider = NULL;

	return WNetAddConnection2(&NetRes, pszPassword, pszUserName, 0);
}

//---------------------------------------------------------------------------
// EnumProcesses_PerfData
//
//  Enumerates all processes in the system using performance data.
//  
//  Parameters:
//	  pszMachineName - name of the computer on which enumerate processes;
//					   this parameter can be NULL
//	  pfnEnumProc    - address of the function to call for each process
//	  lParam	     - additional parameter to pass to the function
//
//  Returns:
//	  TRUE, if successful, FALSE - otherwise.
//
BOOL
WINAPI
EnumProcesses_PerfData(
	IN LPCTSTR pszMachineName,
	IN PFNENUMPROC pfnEnumProc,
	IN LPARAM lParam
	)
{
	_ASSERTE(pfnEnumProc != NULL);

	HINSTANCE hPdh;

	PDH_STATUS (WINAPI * _PdhOpenQuery)(LPCWSTR, DWORD_PTR, HQUERY *);
	PDH_STATUS (WINAPI * _PdhAddCounter)(HQUERY, LPCWSTR, DWORD_PTR, 
		HCOUNTER *);
	PDH_STATUS (WINAPI * _PdhCollectQueryData)(HQUERY);
	PDH_STATUS (WINAPI * _PdhGetRawCounterArray)(HCOUNTER,
		LPDWORD, LPDWORD, PDH_RAW_COUNTER_ITEM_W *);
	PDH_STATUS (WINAPI * _PdhCloseQuery)(HQUERY);
	
	// load PDH.DLL
	hPdh = LoadLibrary(_T("pdh.dll"));
	if (hPdh == NULL)
		return FALSE;

	// find all necessary entry points
	*(FARPROC *)&_PdhOpenQuery =
		GetProcAddress(hPdh, "PdhOpenQueryW");
	*(FARPROC *)&_PdhAddCounter =
		GetProcAddress(hPdh, "PdhAddCounterW");
	*(FARPROC *)&_PdhGetRawCounterArray =
		GetProcAddress(hPdh, "PdhGetRawCounterArrayW");
	*(FARPROC *)&_PdhCollectQueryData =
		GetProcAddress(hPdh, "PdhCollectQueryData");
	*(FARPROC *)&_PdhCloseQuery =
		GetProcAddress(hPdh, "PdhCloseQuery");

	// check if all entry points were found
	if (_PdhOpenQuery == NULL ||
		_PdhAddCounter == NULL ||
		_PdhCollectQueryData == NULL ||
		_PdhGetRawCounterArray == NULL ||
		_PdhCloseQuery == NULL)
	{
		FreeLibrary(hPdh);
		return SetLastError(ERROR_PROC_NOT_FOUND), FALSE;
	}

	PDH_STATUS Status;
	WCHAR szCounterPath[1024];

	Status = PerfFormatCounterPath(hPdh, pszMachineName, szCounterPath, 1024);
	if (Status != ERROR_SUCCESS)
	{
		FreeLibrary(hPdh);
		return SetLastError(Status), FALSE;
	}

	HQUERY hQuery;
	HCOUNTER hCounter;

	// open PDH query
	Status = _PdhOpenQuery(NULL, 0, &hQuery);
	if (Status != ERROR_SUCCESS)
	{
		FreeLibrary(hPdh);
		return SetLastError(Status), FALSE;
	}

	// add process ID counter to the query
	Status = _PdhAddCounter(hQuery, szCounterPath, 0, &hCounter);
	if (Status != ERROR_SUCCESS)
	{
		_PdhCloseQuery(hQuery);
		FreeLibrary(hPdh);
		return SetLastError(Status), FALSE;
	}

	// collect query data
	Status = _PdhCollectQueryData(hQuery);
	if (Status != ERROR_SUCCESS)
	{
		_PdhCloseQuery(hQuery);
		FreeLibrary(hPdh);
		return SetLastError(Status), FALSE;
	}

	DWORD cbSize = 0;
	DWORD cItems = 0;
	HANDLE hHeap = GetProcessHeap();

	// determine size of the array of raw counter values
	Status = _PdhGetRawCounterArray(hCounter, &cbSize, &cItems, NULL);
	if (Status != ERROR_SUCCESS && Status != PDH_MORE_DATA)
	{
		_PdhCloseQuery(hQuery);
		FreeLibrary(hPdh);
		return SetLastError(Status), FALSE;
	}

	// allocate memory for the array
	PDH_RAW_COUNTER_ITEM_W * pRaw =
		(PDH_RAW_COUNTER_ITEM_W *)HeapAlloc(hHeap, 0, cbSize);
	if (pRaw == NULL)
	{
		_PdhCloseQuery(hQuery);
		FreeLibrary(hPdh);
		return SetLastError(ERROR_NOT_ENOUGH_MEMORY), FALSE;
	}

	// retrieve raw counter values
	Status = _PdhGetRawCounterArray(hCounter, &cbSize, &cItems, pRaw);

	// close query handle, we don't need it anymore
	_PdhCloseQuery(hQuery);

	if (Status != ERROR_SUCCESS)
	{
		HeapFree(hHeap, 0, pRaw);
		FreeLibrary(hPdh);
		return SetLastError(Status), FALSE;
	}

	// enumerate processes
	for (DWORD i = 0; i < cItems; i++)
	{
		DWORD dwProcessId = (DWORD)pRaw[i].RawValue.FirstValue;
		LPCTSTR pszProcessName;

#ifdef UNICODE
		pszProcessName = pRaw[i].szName;
#else
		CHAR szProcessName[MAX_PATH];
		WideCharToMultiByte(CP_ACP, 0, pRaw[i].szName, -1,
							szProcessName, MAX_PATH, NULL, NULL);
		pszProcessName = szProcessName;
#endif

		// exclude _Total counter
		if (dwProcessId == 0 && lstrcmp(pszProcessName, _T("_Total")) == 0)
			continue;

		if (!pfnEnumProc(dwProcessId, pszProcessName, lParam))
			break;
	}

	HeapFree(hHeap, 0, pRaw);
	FreeLibrary(hPdh);

    return TRUE;
}

//---------------------------------------------------------------------------
// Q&A unplugged: WMI authentication
//
// WmiSetBlanket
//
//  Sets security blanket on the specified interface pointer to use
//  alternate credentials.
//
//  Parameters:
//	  pInterface - pointer to the interface
//	  pIdentity  - specifies alternate credentials
//
//  Returns:
//	  standard COM return code.
//
//static
//HRESULT
//WmiSetBlanket(
//	IN IUnknown * pInterface,
//	IN SEC_WINNT_AUTH_IDENTITY_W * pIdentity
//	)
//{
//	return CoSetProxyBlanket(pInterface, 
//							 RPC_C_AUTHN_WINNT, 
//							 RPC_C_AUTHZ_NONE, NULL,
//							 RPC_C_AUTHN_LEVEL_CONNECT,
//							 RPC_C_IMP_LEVEL_IMPERSONATE,
//							 pIdentity, 
//							 EOAC_NONE);
//}

//---------------------------------------------------------------------------
// EnumProcesses_Wmi
//
//  Enumerates all processes in the system using WMI interfaces.
//  
//  Parameters:
//	  pszMachineName - name of the computer on which enumerate processes;
//					   this parameter can be NULL
//	  pfnEnumProc    - address of the function to call for each process
//	  lParam	     - additional parameter to pass to the function
//
//  Returns:
//	  TRUE, if successful, FALSE - otherwise.
//
BOOL
WINAPI
EnumProcesses_Wmi(
	IN LPCTSTR pszMachineName,
	IN PFNENUMPROC pfnEnumProc,
	IN LPARAM lParam
	)
{
	_ASSERTE(pfnEnumProc != NULL);

	try
	{
		HRESULT hRes;
		TCHAR szServer[256];
		_bstr_t bstrServer;
		IWbemLocatorPtr spLocator;
		IWbemServicesPtr spServices;
		IEnumWbemClassObjectPtr spEnum;
		IWbemClassObjectPtr spObject;
		ULONG ulCount;

		// create a WBEM locator object
		hRes = CoCreateInstance(__uuidof(WbemLocator), NULL,
							CLSCTX_INPROC_SERVER, __uuidof(IWbemLocator),
							(PVOID *)&spLocator);
		if (FAILED(hRes))
			_com_issue_error(hRes);

		if (pszMachineName == NULL)
			lstrcpy(szServer, _T("root\\cimv2"));
		else
			wsprintf(szServer, _T("%s\\root\\cimv2"), pszMachineName);

		// connect to the services object on the specified machine
		hRes = spLocator->ConnectServer(_bstr_t(szServer), NULL,
										NULL, NULL, 0, NULL, NULL, 
										&spServices);

// Q&A unplugged: WMI authentication
//		
//		_bstr_t bstrUser = _bstr_t("root");
//		_bstr_t bstrPassword = _bstr_t("1");
//
//		SEC_WINNT_AUTH_IDENTITY_W Identity;
//		Identity.Domain = NULL;
//		Identity.DomainLength = 0;
//		Identity.User = bstrUser;
//		Identity.UserLength = bstrUser.length();
//		Identity.Password = bstrPassword;
//		Identity.PasswordLength = bstrPassword.length();
//		Identity.Flags = SEC_WINNT_AUTH_IDENTITY_UNICODE;
//
//		hRes = spLocator->ConnectServer(_bstr_t(szServer), _bstr_t("root"),
//										_bstr_t("1"), NULL, 0, NULL, NULL, 
//										&spServices);

		if (FAILED(hRes))
			_com_issue_error(hRes);

//		hRes = WmiSetBlanket(spServices, &Identity);
//		if (FAILED(hRes))
//			_com_issue_error(hRes);

		// create an enumerator of processes
		hRes = spServices->CreateInstanceEnum(_bstr_t(L"Win32_Process"),
									WBEM_FLAG_SHALLOW|WBEM_FLAG_FORWARD_ONLY,
									NULL, &spEnum);
		if (FAILED(hRes))
			_com_issue_error(hRes);

//		hRes = WmiSetBlanket(spEnum, &Identity);
//		if (FAILED(hRes))
//			_com_issue_error(hRes);

		// walk through all processes
		while (spEnum->Next(WBEM_INFINITE, 1, &spObject, &ulCount) == S_OK)
		{
			_variant_t valId;
			_variant_t valName;

			// retrieve process identifier
			hRes = spObject->Get(L"ProcessId", 0, &valId, NULL, NULL);
			if (FAILED(hRes))
			{
				spObject = NULL;
				continue;
			}

			// retrieve process name
			hRes = spObject->Get(L"Name", 0, &valName, NULL, NULL);
			if (FAILED(hRes))
			{
				spObject = NULL;
				continue;
			}

			_ASSERTE(valId.vt == VT_I4);
			_ASSERTE(valName.vt == VT_BSTR);

			if (!pfnEnumProc(valId.lVal, (LPCTSTR)_bstr_t(valName), lParam))
				break;

			spObject = NULL;
		}
	}
	catch (_com_error& err)
	{
		return SetLastError(err.Error()), FALSE;
	}

	return TRUE;
}
