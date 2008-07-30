// enumproc.h
//
// Enumeration of processes.
//
// $Id: $
//

#ifndef __enumproc_h_included
#define __enumproc_h_included

// callback function prototype
typedef BOOL (CALLBACK * PFNENUMPROC)(
	IN DWORD dwProcessId,
	IN LPCTSTR pszProcessName,
	IN LPARAM lParam
	);

// generic enumeration function prototype
typedef BOOL (WINAPI * PFNENUMPROCESSES)(
	IN LPCTSTR pszMachineName,
	IN PFNENUMPROC pfnEnumProc,
	IN LPARAM lParam
	);

//
// several implementations of the enumeration function
//

BOOL
WINAPI
EnumProcesses_PsApi(
	IN LPCTSTR pszMachineName,
	IN PFNENUMPROC pfnEnumProc,
	IN LPARAM lParam
	);

BOOL
WINAPI
EnumProcesses_ToolHelp(
	IN LPCTSTR pszMachineName,
	IN PFNENUMPROC pfnEnumProc,
	IN LPARAM lParam
	);

BOOL
WINAPI
EnumProcesses_NtApi(
	IN LPCTSTR pszMachineName,
	IN PFNENUMPROC pfnEnumProc,
	IN LPARAM lParam
	);

BOOL
WINAPI
EnumProcesses_PerfData(
	IN LPCTSTR pszMachineName,
	IN PFNENUMPROC pfnEnumProc,
	IN LPARAM lParam
	);

BOOL
WINAPI
EnumProcesses_Wmi(
	IN LPCTSTR pszMachineName,
	IN PFNENUMPROC pfnEnumProc,
	IN LPARAM lParam
	);

DWORD
WINAPI
PerfAuthenticate(
	IN LPCTSTR pszMachineName,
	IN LPCTSTR pszUserName,
	IN LPCTSTR pszPassword
	);


#endif // __enumproc_h_included
