// hungtest.cpp : Defines the entry point for the application.
//

#include "stdafx.h"
#include "resource.h"

CComModule		_Module;

class CMainDlg : public CDialogImpl<CMainDlg>
{
  public:

	enum { IDD = IDD_MAIN };

	BEGIN_MSG_MAP(CMainDlg)
		MESSAGE_HANDLER(WM_INITDIALOG, OnInitDialog)
		COMMAND_HANDLER(IDC_HANG, BN_CLICKED, OnHang)
		COMMAND_HANDLER(IDCANCEL, BN_CLICKED, OnCancel)
	END_MSG_MAP()

	LRESULT OnInitDialog(UINT, WPARAM, LPARAM, BOOL&)
	{
		SetDlgItemInt(IDC_DELAY, 30);
		CenterWindow();
		return TRUE;
	}

	LRESULT OnHang(WORD, WORD, HWND, BOOL&)
	{
		UINT nDelay = GetDlgItemInt(IDC_DELAY);
		Sleep(nDelay * 1000);
		return 0;
	}

	LRESULT OnCancel(WORD, WORD, HWND, BOOL&)
	{
		return EndDialog(0);
	}
};

//---------------------------------------------------------------------------
// WinMain
//
//  Program entry point.
//
//  Parameters:
//    hInstance     - instance handle
//    hPrevInstance - previous instance handle
//    pszCmdLine    - command line
//    nShowCmd	    - show command
//
//  Returns:
//    the return value is the exit code of the process.
//
EXTERN_C
int
WINAPI
_tWinMain(
    IN HINSTANCE hInstance,
    IN HINSTANCE hPrevInstance,
    IN PTSTR pszCmdLine,
    IN int nShowCmd
    )
{
    _UNUSED(hPrevInstance);
	_UNUSED(pszCmdLine);
	_UNUSED(nShowCmd);

    _Module.Init(NULL, hInstance);

    CMainDlg dlgMain;
	dlgMain.DoModal();

    _Module.Term();
    return 0;
}
