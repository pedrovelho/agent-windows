// pview.cpp

#include "stdafx.h"
#include "pview.h"
#include "mainfrm.h"

//BEGIN_MESSAGE_MAP(CApp, CWinApp)
	//{{AFX_MSG_MAP(CApp)
	//}}AFX_MSG_MAP
//END_MESSAGE_MAP()

CApp g_theApp;

//---------------------------------------------------------------------------
// InitInstance
//
BOOL
CApp::InitInstance()
{
	CoInitialize(NULL);

	CoInitializeSecurity(
		  NULL,
		  -1,
		  NULL,
		  NULL,
		  RPC_C_AUTHN_LEVEL_CONNECT,
		  RPC_C_IMP_LEVEL_IMPERSONATE,
		  NULL,
		  EOAC_NONE,
		  0);

	SetRegistryKey(_T("RSDN"));

	CMainFrame * pFrame = new CMainFrame;
	m_pMainWnd = pFrame;

	pFrame->LoadFrame(IDR_MAINFRAME, WS_OVERLAPPEDWINDOW, NULL, NULL);

	pFrame->ShowWindow(SW_SHOW);
	pFrame->UpdateWindow();

	return TRUE;
}

//---------------------------------------------------------------------------
// ExitInstance
//
int
CApp::ExitInstance()
{
	CoUninitialize();
	return CWinApp::ExitInstance();
}
