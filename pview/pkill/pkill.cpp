// pkill.cpp
//
// Kills the specified process or process tree.
//
// $Id: $
//

#include "stdafx.h"
#include "killproc.h"

#include <lm.h>

//---------------------------------------------------------------------------
// PrintUsage
//
//  Prints program usage information.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  one.
//
int
PrintUsage()
{
	_tprintf(_T("Usage:\n"));
	_tprintf(_T("\n"));
	_tprintf(_T("pkill [-9] <pid>\n"));
	return 1;
}

//---------------------------------------------------------------------------
// _tmain
//
//  Program entry point.
//
//  Parameters:
//    none.
//
//  Returns:
//	  program's exit code.
//
int
_tmain()
{
	LPCTSTR pszCmdLine = GetCommandLine();
	DWORD dwProcessId = 0;
	BOOL bTree = TRUE;

	// skip process name in the beginning of the command line
	TCHAR chSep = _T(' ');
	if (*pszCmdLine == _T('\"'))
	{
		chSep = _T('\"');
		pszCmdLine++;
	}

	while (*pszCmdLine != 0 && *pszCmdLine != chSep)
		pszCmdLine++;
	pszCmdLine++;

	// skip whitespace
	while (*pszCmdLine == _T(' '))
		pszCmdLine++;

	if (*pszCmdLine == 0)
		return PrintUsage();

	// check if -9 switch is present
	if (pszCmdLine[0] == _T('-'))
	{
		if (pszCmdLine[1] == _T('9') &&
			pszCmdLine[2] == _T(' '))
		{
			bTree = FALSE;
			pszCmdLine += 3;
		}
		else
			return PrintUsage();

		// skip whitespace
		while (*pszCmdLine == _T(' '))
			pszCmdLine++;

		if (*pszCmdLine == 0)
			return PrintUsage();
	}

	// parse process identifier
	while (*pszCmdLine >= _T('0') &&
		   *pszCmdLine <= _T('9'))
	{
		dwProcessId = dwProcessId * 10 + (*pszCmdLine - _T('0'));
		pszCmdLine++;
	}

	if (!KillProcessEx(dwProcessId, bTree))
	{
		_tprintf(_T("Failed to terminate process; error %d\n"), 
				 GetLastError());
		return 1;
	}

	return 0;
}

#ifndef _DEBUG

//---------------------------------------------------------------------------
// _tprintf
//
//  printf implementation for Release build to avoid inclusion of CRT stuff.
//
//  Parameters:
//	  fmt - format string
//	  ... - optional parameters
//
//  Returns:
//    number of characters printed.
//
int
__cdecl
_tprintf(
	const _TCHAR * fmt,
	...
	)
{
	va_list va;
	va_start(va, fmt);

	_TCHAR szBuffer[1024];

	int ret = wvsprintf(szBuffer, fmt, va);

	va_end(va);

	if (!WriteFile(GetStdHandle(STD_OUTPUT_HANDLE), szBuffer, ret, 
				   (DWORD *)&ret, NULL))
		return -1;

	return ret;
}

#ifndef _UNICODE

//---------------------------------------------------------------------------
// mainCRTStartup
//
//  The real process entry point.
//
//  Parameters:
//    none.
//
//  Returns:
//	  this function does not return to the caller.
//
extern "C"
void 
__cdecl 
mainCRTStartup()
{
	ExitProcess(main());
}

#else

//---------------------------------------------------------------------------
// wmainCRTStartup
//
//  The real process entry point.
//
//  Parameters:
//    none.
//
//  Returns:
//	  this function does not return to the caller.
//
extern "C"
void 
__cdecl 
wmainCRTStartup()
{
	ExitProcess(wmain());
}

#endif

#endif
