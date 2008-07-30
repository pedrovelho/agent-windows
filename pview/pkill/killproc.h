// killproc.h
//
// Termination of processes.
//
// $Id: $
//

#ifndef __killproc_h_included
#define __killproc_h_included

// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the EMPTY_DLL_EXPORTS
// symbol defined on the command line. this symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// EMPTY_DLL_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef PKILL_DLL_EXPORTS
#define PKILL_DLL_API __declspec(dllexport)
#else
#define PKILL_DLL_API __declspec(dllimport)
#endif

//EMPTY_DLL_API int fnempty_dll(void);

extern "C"
BOOL
PKILL_DLL_API
WINAPI
KillProcess(
	IN DWORD dwProcessId
	);

extern "C"
BOOL
PKILL_DLL_API
WINAPI
KillProcessEx(
	IN DWORD dwProcessId,
	IN BOOL bTree
	);

#endif // __killproc_h_included
