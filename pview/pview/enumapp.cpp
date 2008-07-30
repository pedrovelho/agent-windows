// enumapp.cpp
//
// Enumeration of applications.
//
// $Id: $
//

#include "stdafx.h"
#include "enumapp.h"

typedef struct _ENUMAPPDATA {
	LPARAM		lParam;
	PFNENUMAPP	pfnEnumApp;
} ENUMAPPDATA, * PENUMAPPDATA;

//---------------------------------------------------------------------------
// EnumWindowsCallback
//
//  This function is called for each top-level window in the system. The
//  function decides whether the window represents a separate application
//	or not, and if it does, the function calls the application-defined
//	callback function.
//
//  Parameters:
//	  hWnd   - window handle
//	  lParam - contains a pointer to the ENUMAPPDATA structure
//
//  Returns:
//	  TRUE - to continue enumeration, FALSE - to stop enumeration.
//
static
BOOL
CALLBACK
EnumWindowsCallback(
	IN HWND hWnd,
	IN LPARAM lParam
	)
{
	PENUMAPPDATA pEnumData = (PENUMAPPDATA)lParam;
	_ASSERTE(_CrtIsValidPointer(pEnumData, sizeof(ENUMAPPDATA), 1));

	if (!IsWindowVisible(hWnd) || GetWindow(hWnd, GW_OWNER) != NULL)
		return TRUE;

	TCHAR szClassName[80];
	GetClassName(hWnd, szClassName, 80);

	if (lstrcmpi(szClassName, _T("Progman")) == 0)
		return TRUE;

	// obtain window 
	TCHAR szText[256];
	DWORD cchText = GetWindowText(hWnd, szText, 256);

	if (cchText == 0)
		return TRUE;

	HICON hIcon = NULL;
	if (SendMessageTimeout(hWnd, WM_GETICON, ICON_SMALL, 0, 
							SMTO_ABORTIFHUNG|SMTO_BLOCK, 1000, 
							(DWORD_PTR *)&hIcon))
	{
		if (hIcon == NULL)
		{
			if (!SendMessageTimeout(hWnd, WM_GETICON, ICON_BIG, 0, 
								SMTO_ABORTIFHUNG|SMTO_BLOCK, 1000, 
								(DWORD_PTR *)&hIcon))
				hIcon = NULL;
		}
	}
	else
		hIcon = NULL;

	if (hIcon == NULL)
		hIcon = (HICON)GetClassLong(hWnd, GCL_HICONSM);

	if (hIcon == NULL)
		hIcon = (HICON)GetClassLong(hWnd, GCL_HICON);

	if (hIcon == NULL)
		hIcon = LoadIcon(NULL, IDI_APPLICATION);

	return pEnumData->pfnEnumApp(hWnd, szText, hIcon, pEnumData->lParam);
}

//---------------------------------------------------------------------------
// EnumApplications
//
//  Enumerates applications on the local computer.
//
//  Parameters:
//    pfnEnumApp - pointer to an application-defined function to call
//	               for each application
//    lParam	 - parameter to pass to the function
//
//  Returns:
//	  TRUE, if successful, FALSE - otherwise.
//  
BOOL
WINAPI
EnumApplications(
    IN PFNENUMAPP pfnEnumApp,
    IN LPARAM lParam
    )
{
	_ASSERTE(pfnEnumApp != NULL);

	ENUMAPPDATA EnumData;
	EnumData.pfnEnumApp = pfnEnumApp;
	EnumData.lParam = lParam;

	return EnumWindows(EnumWindowsCallback, (LPARAM)&EnumData);
}
