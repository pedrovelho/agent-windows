// nttask16.cpp
//
// Demonstrates enumeration of 16-bit tasks on Windows NT
//
// $Id: $
//

#define STRICT
#include <windows.h>
#include <stdio.h>
#include <vdmdbg.h>

#pragma comment(lib, "vdmdbg.lib")

BOOL
CALLBACK
TaskEnumProcEx(
	IN DWORD dwThreadId,
	IN WORD hMod16,
	IN WORD hTask16,
    IN LPSTR pszModName,
	IN LPSTR pszFileName,
	IN LPARAM lParam
	)
{
	printf("\tThread ID:\t%u (0x%X)\n", dwThreadId, dwThreadId);
	printf("\tModule handle:\t0x%04X\n", hMod16);
	printf("\tTask handle:\t0x%04X\n", hTask16);
	printf("\tModule Name:\t%s\n", pszModName);
	printf("\tFile Name:\t%s\n\n", pszFileName);

	return FALSE;
} 

BOOL
CALLBACK
ProcessEnumProc(
	IN DWORD dwProcessId,
	IN DWORD dwAttrib,
	IN LPARAM lParam
	)
{
	printf("VDM Process ID: %u (0x%X)\n", dwProcessId, dwProcessId);
	printf("Attributes: 0x%08X\n", dwAttrib);

	VDMEnumTaskWOWEx(dwProcessId, TaskEnumProcEx, 0);
	
	return FALSE;
}

int
main(
	int argc,
	char * argv[]
	)
{
	VDMEnumProcessWOW(ProcessEnumProc, 0);

	return 0;
}
