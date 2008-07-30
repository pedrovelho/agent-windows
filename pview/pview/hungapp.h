// hungapp.h
//
// Applicationg hangup detection.
//
// $Id: $
//

#ifndef __hungapp_h_included
#define __hungapp_h_included

typedef BOOL (WINAPI * PFNISAPPHUNG)(
	IN HWND hWnd,
	OUT PBOOL pbHung
	);


BOOL 
WINAPI
IsAppHung_SMTO(
	IN HWND hWnd,
	OUT PBOOL pbHung
	);

BOOL
WINAPI
IsAppHung_Undoc(
	IN HWND hWnd,
	OUT PBOOL pbHung
	);


#endif // __hungapp_h_included
