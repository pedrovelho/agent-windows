// killapp.h
//
// Termination of applications.
//
// $Id: $
//

#ifndef __killapp_h_included
#define __killapp_h_included

typedef int (CALLBACK * PFNWAITCALLBACK)(LPARAM);

#define KILLAPP_TERMINATE	-1
#define KILLAPP_WAIT		 0
#define KILLAPP_CANCEL		 1

BOOL
WINAPI
KillApplication(
	IN HWND hWnd,
	IN PFNWAITCALLBACK pfnWaitCallback,
	IN LPARAM lParam
	);

#endif // __killapp_h_included
