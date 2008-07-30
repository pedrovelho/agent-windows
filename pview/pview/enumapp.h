// enumapp.h
//
// Enumeration of applications.
//
// $Id: $
//

#ifndef __enumapp_h_included
#define __enumapp_h_included

// callback function prototype
typedef BOOL (CALLBACK * PFNENUMAPP)(
	IN HWND hWnd,
	IN LPCTSTR pszName,
	IN HICON hIcon,
	IN LPARAM lParam
	);

// application enumeration function
BOOL
WINAPI
EnumApplications(
    IN PFNENUMAPP pfnEnumApp,
    IN LPARAM lParam
    );


#endif // __enumapp_h_included
