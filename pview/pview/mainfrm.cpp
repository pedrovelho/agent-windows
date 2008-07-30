// mainfrm.cpp

#include "stdafx.h"
#include "pview.h"
#include "mainfrm.h"

#include "enumproc.h"
#include "enumapp.h"
#include "killproc.h"
#include "killapp.h"
#include "hungapp.h"

#include "secedit.h"

BEGIN_MESSAGE_MAP(CMainFrame, CFrameWnd)
	//{{AFX_MSG_MAP(CMainFrame)
	ON_WM_CONTEXTMENU()
	ON_WM_CREATE()
	ON_WM_DESTROY()
	ON_WM_SETFOCUS()
	ON_COMMAND(ID_VIEW_REFRESH, OnViewRefresh)
	ON_COMMAND(ID_VIEW_APPLICATIONS, OnViewApplications)
	ON_UPDATE_COMMAND_UI(ID_VIEW_APPLICATIONS, OnViewApplications_Update)
	ON_COMMAND(ID_VIEW_PROCESSES, OnViewProcesses)
	ON_UPDATE_COMMAND_UI(ID_VIEW_PROCESSES, OnViewProcesses_Update)
	ON_COMMAND(ID_VIEW_16BIT, OnView16bit)
	ON_UPDATE_COMMAND_UI(ID_VIEW_16BIT, OnView16bit_Update)
	ON_COMMAND(ID_OPTIONS_ENUMPROC_TOOLHELP, OnOptionsEnumprocToolhelp)
	ON_UPDATE_COMMAND_UI(ID_OPTIONS_ENUMPROC_TOOLHELP, OnOptionsEnumprocToolhelp_Update)
	ON_COMMAND(ID_OPTIONS_ENUMPROC_PSAPI, OnOptionsEnumprocPsapi)
	ON_UPDATE_COMMAND_UI(ID_OPTIONS_ENUMPROC_PSAPI, OnOptionsEnumprocPsapi_Update)
	ON_COMMAND(ID_OPTIONS_ENUMPROC_NTAPI, OnOptionsEnumprocNtapi)
	ON_UPDATE_COMMAND_UI(ID_OPTIONS_ENUMPROC_NTAPI, OnOptionsEnumprocNtapi_Update)
	ON_COMMAND(ID_OPTIONS_ENUMPROC_PERFDATA, OnOptionsEnumprocPerfdata)
	ON_UPDATE_COMMAND_UI(ID_OPTIONS_ENUMPROC_PERFDATA, OnOptionsEnumprocPerfdata_Update)
	ON_UPDATE_COMMAND_UI(ID_OPTIONS_DEBUG, OnOptionsDebug_Update)
	ON_COMMAND(ID_OPTIONS_DEBUG, OnOptionsDebug)
	ON_COMMAND(ID_OPTIONS_ENUMPROC_WMI, OnOptionsEnumprocWmi)
	ON_UPDATE_COMMAND_UI(ID_OPTIONS_ENUMPROC_WMI, OnOptionsEnumprocWmi_Update)
	ON_COMMAND(ID_ACTION_CONNECT, OnActionConnect)
	ON_UPDATE_COMMAND_UI(ID_ACTION_CONNECT, OnActionConnect_Update)
	ON_COMMAND(ID_ACTION_DISCONNECT, OnActionDisconnect)
	ON_UPDATE_COMMAND_UI(ID_ACTION_DISCONNECT, OnActionDisconnect_Update)
	ON_COMMAND(ID_ACTION_KILLPROC, OnActionKillProc)
	ON_UPDATE_COMMAND_UI(ID_ACTION_KILLPROC, OnActionKillProc_Update)
	ON_COMMAND(ID_ACTION_KILLTREE, OnActionKillTree)
	ON_UPDATE_COMMAND_UI(ID_ACTION_KILLTREE, OnActionKillTree_Update)
	ON_COMMAND(ID_VIEW_SPEED_HIGH, OnViewSpeedHigh)
	ON_UPDATE_COMMAND_UI(ID_VIEW_SPEED_HIGH, OnViewSpeedHigh_Update)
	ON_COMMAND(ID_VIEW_SPEED_NORMAL, OnViewSpeedNormal)
	ON_UPDATE_COMMAND_UI(ID_VIEW_SPEED_NORMAL, OnViewSpeedNormal_Update)
	ON_COMMAND(ID_VIEW_SPEED_LOW, OnViewSpeedLow)
	ON_UPDATE_COMMAND_UI(ID_VIEW_SPEED_LOW, OnViewSpeedLow_Update)
	ON_COMMAND(ID_VIEW_SPEED_PAUSED, OnViewSpeedPaused)
	ON_UPDATE_COMMAND_UI(ID_VIEW_SPEED_PAUSED, OnViewSpeedPaused_Update)
	ON_WM_TIMER()
	ON_COMMAND(ID_ACTION_SECURITY, OnActionSecurity)
	ON_UPDATE_COMMAND_UI(ID_ACTION_SECURITY, OnActionSecurity_Update)
	ON_COMMAND(ID_OPTIONS_HANGUP_SMTO, OnOptionsHangupSmto)
	ON_UPDATE_COMMAND_UI(ID_OPTIONS_HANGUP_SMTO, OnOptionsHangupSmto_Update)
	ON_COMMAND(ID_OPTIONS_HANGUP_UNDOC, OnOptionsHangupUndoc)
	ON_UPDATE_COMMAND_UI(ID_OPTIONS_HANGUP_UNDOC, OnOptionsHangupUndoc_Update)
	ON_COMMAND(ID_ACTION_KILLAPP, OnActionKillApp)
	ON_UPDATE_COMMAND_UI(ID_ACTION_KILLAPP, OnActionKillApp_Update)
	ON_WM_SYSCOLORCHANGE()
	//}}AFX_MSG_MAP
	ON_NOTIFY(LVN_COLUMNCLICK, AFX_IDW_PANE_FIRST, OnList_ColumnClick)
	ON_NOTIFY(LVN_DELETEITEM, AFX_IDW_PANE_FIRST, OnList_DeleteItem)
END_MESSAGE_MAP()

//---------------------------------------------------------------------------
// CMainFrame
//
//  Constructor, initializes the object.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
CMainFrame::CMainFrame()
{
	m_nEnumProcMethod = ENUMPROCESSES_PSAPI;
	m_bProcesses = -1;
	m_nSortOrder = -1;
	m_nAppsSortOrder = -1;
	m_bShow16Bit = FALSE;
	m_hVdmDbg = NULL;
	m_pfnVDMEnumTaskWOWEx = NULL;
	m_pfnVDMTerminateTaskWOW = NULL;
	m_hPDH = NULL;
	m_hPSAPI = NULL;
	m_bWmiAvailable = NULL;
	m_nRefreshPeriod = UPDATE_PERIOD_NORMAL;
	m_bSedAvailable = FALSE;
	m_pfnIsAppHung = IsAppHung_SMTO;
	m_dwWaitStart = 0;

	m_osvi.dwOSVersionInfoSize = sizeof(m_osvi);
	_VERIFY(GetVersionEx(&m_osvi));
}

//---------------------------------------------------------------------------
// ~CMainFrame
//
//  Destructor, performs necessary cleanup.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  none.
//
CMainFrame::~CMainFrame()
{
}

//---------------------------------------------------------------------------
// PreCreateWindow
//
BOOL
CMainFrame::PreCreateWindow(
	CREATESTRUCT& cs
	)
{
	if (!CFrameWnd::PreCreateWindow(cs))
		return FALSE;

	cs.dwExStyle &= ~WS_EX_CLIENTEDGE;
	cs.lpszClass = AfxRegisterWndClass(0, NULL, NULL, 
									   AfxGetApp()->LoadIcon(IDR_MAINFRAME));

	return TRUE;
}

//---------------------------------------------------------------------------
// OnContextMenu
//
//  Handles WM_CONTEXTMENU message. Displays a shortcut menu for the
//  list view control.
//
//  Parameters:
//	  pWnd  - pointer to the window that was right-clicked
//	  point - screen coordinates of the cursor at the moment of the click
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnContextMenu(
	CWnd * pWnd,
	CPoint point
	) 
{
	if (pWnd != &m_wndView)
		return;

	int nSel = m_wndView.GetNextItem(-1, LVNI_SELECTED);
	if (nSel == -1)
		return;

	if (point.x == -1 && point.y == -1)
	{
		RECT rect;
		m_wndView.GetItemRect(nSel, &rect, LVIR_BOUNDS);
		m_wndView.ClientToScreen(&rect);

		point.x = rect.left + 1;
		point.y = rect.bottom + 1;
	}

	CMenu menu;
	_VERIFY(menu.LoadMenu(IDR_POPUPS));

	int nMenu = m_bProcesses ? 0 : 1;

	menu.GetSubMenu(nMenu)->TrackPopupMenu(TPM_LEFTALIGN|TPM_LEFTBUTTON,
								point.x, point.y, this);
}

//---------------------------------------------------------------------------
// OnCreate
//
//  Handles WM_CREATE message. Initializes the window.
//
//  Parameters:
//	  pCreateStruct - pointer to CREATESTRUCT structure that holds window
//					  creation parameters
//
//  Returns:
//	  zero if successful, -1 if failed.
//
int
CMainFrame::OnCreate(
	CREATESTRUCT * pCreateStruct
	)
{
	OnSysColorChange();

	RECT rcEmpty;
	SetRectEmpty(&rcEmpty);

	if (CFrameWnd::OnCreate(pCreateStruct) == -1)
		return -1;

	// create status bar
	UINT nInd = ID_SEPARATOR;

	if (!m_wndStatusBar.Create(this) ||
		!m_wndStatusBar.SetIndicators(&nInd, 1))
		return -1;

	// create image list
	if (!m_ImageList.Create(16, 16, ILC_COLOR32|ILC_MASK, 16, 16))
		return -1;

	// insert the default application icon into the image list
	m_ImageList.Add(LoadIcon(NULL, IDI_APPLICATION));

	// locate VDMEnumTaskWOWEx entry point in VDMDBG.DLL
	if (m_osvi.dwPlatformId == VER_PLATFORM_WIN32_NT)
	{
		m_hVdmDbg = LoadLibrary(_T("vdmdbg.dll"));
		if (m_hVdmDbg != NULL)
		{
			m_pfnVDMEnumTaskWOWEx = 
				(VDMENUMTASKWOWEXPROC)GetProcAddress(m_hVdmDbg, 
												     "VDMEnumTaskWOWEx");
			m_pfnVDMTerminateTaskWOW =
				(VDMTERMINATETASKINWOWPROC)GetProcAddress(m_hVdmDbg,
													 "VDMTerminateTaskWOW");
		}
	}

	// try to load PSAPI.DLL; this DLL might be not installed on NT 4, so
	// we want to check this
	m_hPSAPI = LoadLibrary(_T("psapi.dll"));

	// try to load PDH.DLL; this DLL might be not installed on NT 4, so
	// we want to check this; also, preloading PDH.DLL greatly improves
	// speed of the corresponding process enumeration method
	m_hPDH = LoadLibrary(_T("pdh.dll"));

	// check for WMI availability
	IWbemLocator * pLocator = NULL;
	HRESULT hRes = CoCreateInstance(__uuidof(WbemLocator), NULL, 
									CLSCTX_INPROC_SERVER,
									__uuidof(IWbemLocator), 
									(PVOID *)&pLocator);
	if (SUCCEEDED(hRes))
	{
		pLocator->Release();
		m_bWmiAvailable = TRUE;
	}
	else
	{
		m_bWmiAvailable = FALSE;
	}

	// check availability of a security editor
	m_bSedAvailable = CProcessSecInfo::IsDaclEditorAvailable();

	// initialize view parameters from the registry
	m_nEnumProcMethod = (int)g_theApp.GetProfileInt(_T("Settings"), 
									_T("EnumProcessesMethod"), 
									ENUMPROCESSES_INVALID);

	if (m_nEnumProcMethod == ENUMPROCESSES_PSAPI &&
		m_hPSAPI == NULL)
		m_nEnumProcMethod = ENUMPROCESSES_INVALID;

	if (m_nEnumProcMethod == ENUMPROCESSES_PERFDATA &&
		m_hPDH == NULL)
		m_nEnumProcMethod = ENUMPROCESSES_INVALID;
	
	if (m_nEnumProcMethod == ENUMPROCESSES_WMI &&
		!m_bWmiAvailable)
		m_nEnumProcMethod = ENUMPROCESSES_INVALID;

	if (m_nEnumProcMethod == ENUMPROCESSES_INVALID)
	{
		if (m_osvi.dwPlatformId == VER_PLATFORM_WIN32_WINDOWS)
			m_nEnumProcMethod = ENUMPROCESSES_TOOLHELP;
		else
			m_nEnumProcMethod = ENUMPROCESSES_NTAPI;
	}

	int bProcesses = (int)g_theApp.GetProfileInt(_T("Settings"),
									_T("ProcessesMode"), 1);
	m_nSortOrder = (int)g_theApp.GetProfileInt(_T("Settings"),
									_T("ProcessesSortOrder"), -1);
	m_nAppsSortOrder = (int)g_theApp.GetProfileInt(_T("Settings"),
									_T("ApplicationsSortOrder"), -1);
	m_bShow16Bit = (BOOL)g_theApp.GetProfileInt(_T("Settings"),
									_T("Show16Bit"), FALSE);

	if (m_pfnVDMEnumTaskWOWEx == NULL ||
		m_pfnVDMTerminateTaskWOW == NULL)
		m_bShow16Bit = FALSE;

	m_nRefreshPeriod = g_theApp.GetProfileInt(_T("Settings"),
									_T("UpdateSpeed"), 
									UPDATE_PERIOD_NORMAL);

	if (m_nRefreshPeriod != UPDATE_PERIOD_PAUSED)
		SetTimer(1, m_nRefreshPeriod, NULL);

	int bHung = (int)g_theApp.GetProfileInt(_T("Settings"),
									_T("IsAppHung"), 0);
	if (bHung == 0)
		m_pfnIsAppHung = IsAppHung_SMTO;
	else
		m_pfnIsAppHung = IsAppHung_Undoc;

	PBYTE pData = NULL;
	UINT cbData = 0;
	int * pnValues = NULL;

	if (g_theApp.GetProfileBinary(_T("Settings"), _T("ListView"),
								  &pData, &cbData))
	{
		if (cbData == 8 * sizeof(int))
		{
			pnValues = (int *)pData;
			m_nProcColWidth[0] = pnValues[0];
			m_nProcColWidth[1] = pnValues[1];
			m_nAppsColWidth[0] = pnValues[2];
			m_nAppsColWidth[1] = pnValues[3];
			m_nProcColOrder[0] = pnValues[4];
			m_nProcColOrder[1] = pnValues[5];
			m_nAppsColOrder[0] = pnValues[6];
			m_nAppsColOrder[1] = pnValues[7];
		}

		delete[] pData;
	}

	if (pnValues == NULL)
	{
		m_nProcColWidth[0] = LVSCW_AUTOSIZE_USEHEADER;
		m_nProcColWidth[1] = LVSCW_AUTOSIZE_USEHEADER;
		m_nAppsColWidth[0] = LVSCW_AUTOSIZE_USEHEADER;
		m_nAppsColWidth[1] = LVSCW_AUTOSIZE_USEHEADER;
		m_nProcColOrder[0] = 0;
		m_nProcColOrder[1] = 1;
		m_nAppsColOrder[0] = 0;
		m_nAppsColOrder[1] = 1;
	}

	if (bProcesses)
		OnViewProcesses();
	else
		OnViewApplications();

	return 0;
}

//---------------------------------------------------------------------------
// OnDestroy
//
//  Handles WM_DESTROY message. Saves view settings in the registry.
//
//  Parameters:
//	  none.
//
//	Returns:
//	  no return value.
//
void
CMainFrame::OnDestroy() 
{
	SaveViewSettings();

	int bHung = m_pfnIsAppHung != IsAppHung_SMTO;

	g_theApp.WriteProfileInt(_T("Settings"), _T("EnumProcessesMethod"),
							 m_nEnumProcMethod);
	g_theApp.WriteProfileInt(_T("Settings"), _T("ProcessesMode"),
							 m_bProcesses);
	g_theApp.WriteProfileInt(_T("Settings"), _T("ProcessesSortOrder"),
							 m_nSortOrder);
	g_theApp.WriteProfileInt(_T("Settings"), _T("ApplicationsSortOrder"),
							 m_nAppsSortOrder);
	g_theApp.WriteProfileInt(_T("Settings"), _T("Show16Bit"),
							 m_bShow16Bit);
	g_theApp.WriteProfileInt(_T("Settings"), _T("UpdateSpeed"),
							 m_nRefreshPeriod);
	g_theApp.WriteProfileInt(_T("Settings"), _T("IsAppHung"),
							 bHung);

	int nValues[8];
	nValues[0] = m_nProcColWidth[0];
	nValues[1] = m_nProcColWidth[1];
	nValues[2] = m_nAppsColWidth[0];
	nValues[3] = m_nAppsColWidth[1];
	nValues[4] = m_nProcColOrder[0];
	nValues[5] = m_nProcColOrder[1];
	nValues[6] = m_nAppsColOrder[0];
	nValues[7] = m_nAppsColOrder[1];
	
	g_theApp.WriteProfileBinary(_T("Settings"), _T("ListView"),
								(LPBYTE)nValues, sizeof(nValues));

	if (m_hVdmDbg != NULL)
		_VERIFY(FreeLibrary(m_hVdmDbg));

	m_hVdmDbg = NULL;
	m_pfnVDMEnumTaskWOWEx = NULL;
	m_pfnVDMTerminateTaskWOW = NULL;

	if (m_hPSAPI != NULL)
		_VERIFY(FreeLibrary(m_hPSAPI));

	if (m_hPDH != NULL)
		_VERIFY(FreeLibrary(m_hPDH));

	CFrameWnd::OnDestroy();
}

//---------------------------------------------------------------------------
// OnSetFocus
//
//	Handles WM_SETFOCUS message. Passes the focus to the embedded view
//	window.
//
//  Parameters:
//	  pOldWnd - pointer to the window that looses the focus
//
//  Returns:
//	  no return value.
//
void 
CMainFrame::OnSetFocus(
	CWnd * pOldWnd
	)
{
	_UNUSED(pOldWnd);

	// forward focus to the view window
	m_wndView.SetFocus();
}

//---------------------------------------------------------------------------
// OnSysColorChange
//
//  Handles WM_SYSCOLORCHANGE message. Reloads any bitmaps that depends on
//	the current color scheme.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void CMainFrame::OnSysColorChange() 
{
	CFrameWnd::OnSysColorChange();
	
	if (m_bmSortUp.m_hObject != NULL)
		_VERIFY(m_bmSortUp.DeleteObject());
	if (m_bmSortDown.m_hObject != NULL)
		_VERIFY(m_bmSortDown.DeleteObject());

	_VERIFY(m_bmSortUp.LoadMappedBitmap(IDB_SORT_UP));
	_VERIFY(m_bmSortDown.LoadMappedBitmap(IDB_SORT_DOWN));		
}

//---------------------------------------------------------------------------
// OnTimer
//
//  Handles WM_TIMER message. Refreshes the view on timer.
//
//  Parameters:
//	  nIDEvent - timer identifier
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnTimer(
	UINT nIDEvent
	) 
{
	_UNUSED(nIDEvent);

	_ASSERTE(nIDEvent == 1);

	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnActionConnect
//
//  Handles Action|Connect to anothe computer... menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnActionConnect()
{
	IMalloc * pMalloc = NULL;
	HRESULT hRes;

	hRes = SHGetMalloc(&pMalloc);
	if (FAILED(hRes))
		AfxThrowOleException((SCODE)hRes);

	LPITEMIDLIST pidl, pidlRoot;
	BROWSEINFO bi;
	TCHAR szTitle[256];
	TCHAR szComputer[256];

	hRes = SHGetSpecialFolderLocation(m_hWnd, CSIDL_NETWORK, &pidlRoot);
	if (FAILED(hRes))
	{
		pMalloc->Release();
		AfxThrowOleException((SCODE)hRes);
	}

	AfxLoadString(IDS_BROWSE_TITLE, szTitle, countof(szTitle));

	memset(&bi, 0, sizeof(bi));

	bi.hwndOwner = m_hWnd;
	bi.lpszTitle = szTitle;
	bi.ulFlags = BIF_BROWSEFORCOMPUTER|BIF_EDITBOX;
	bi.pidlRoot = pidlRoot;
	bi.pszDisplayName = szComputer;

	pidl = SHBrowseForFolder(&bi);

	pMalloc->Free(pidlRoot);
	if (pidl != NULL)
		pMalloc->Free(pidl);
	pMalloc->Release();

	if (pidl == NULL)
		return;

	if (szComputer[0] != _T('\\'))
	{
		m_strMachineName = _T("\\\\");
		m_strMachineName += szComputer;
	}
	else
		m_strMachineName = szComputer;

	if (m_nEnumProcMethod != ENUMPROCESSES_WMI &&
		m_nEnumProcMethod != ENUMPROCESSES_PERFDATA)
	{
		if (m_bWmiAvailable)
			m_nEnumProcMethod = ENUMPROCESSES_WMI;
		else if (m_hPDH != NULL)
			m_nEnumProcMethod = ENUMPROCESSES_PERFDATA;
		else
			_ASSERTE(0);
	}

	if (m_nRefreshPeriod != UPDATE_PERIOD_PAUSED)
	{
		KillTimer(1);
		m_nRefreshPeriod = UPDATE_PERIOD_PAUSED;
	}

	m_bShow16Bit = FALSE;

//  Q&A unplugged: PDH authentication
//	PerfAuthenticate(m_strMachineName, _T("root"), _T("1"));

	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnActionConnect_Update
//
//  Updates the state of Computer|Connect to another computer... menu
//  command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void 
CMainFrame::OnActionConnect_Update(
	CCmdUI * pCmdUI
	) 
{
	pCmdUI->Enable(m_bProcesses && (m_bWmiAvailable || m_hPDH != NULL));
}

//---------------------------------------------------------------------------
// OnActionDisconnect
//
//  Handles Action|Disconnect menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnActionDisconnect() 
{
	m_strMachineName.Empty();
	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnActionDisconnect_Update
//
//  Updates the state of Action|Disconnect menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnActionDisconnect_Update(
	CCmdUI * pCmdUI
	)
{
	TCHAR szFormat[256];
	TCHAR szText[256];
	UINT nFormat;
	
	if (!m_strMachineName.IsEmpty())
	{
		pCmdUI->Enable(TRUE);
		nFormat = IDS_DISCONNECT_FROM;
	}
	else
	{
		pCmdUI->Enable(FALSE);
		nFormat = IDS_DISCONNECT;
	}

	AfxLoadString(nFormat, szFormat, countof(szFormat));
	wsprintf(szText, szFormat, m_strMachineName);

	pCmdUI->SetText(szText);
}

//---------------------------------------------------------------------------
// OnActionKillProc
//
//  Handles Action|End Process menu command.
// 
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnActionKillProc()
{
	TCHAR szMessage[1024];
	TCHAR szTitle[256];

	AfxLoadString(IDS_KILLPROC_WARNING, szMessage, countof(szMessage));
	AfxLoadString(IDS_WARNING_TITLE, szTitle, countof(szTitle));

	if (MessageBox(szMessage, szTitle, MB_YESNO|MB_ICONEXCLAMATION) != IDYES)
		return;

	int nSel = m_wndView.GetNextItem(-1, LVNI_SELECTED);
	_ASSERTE(nSel != -1);

	CItemData * pData = (CItemData *)m_wndView.GetItemData(nSel);
	_ASSERTE(_CrtIsValidHeapPointer(pData));

	if (pData->dwWowTaskId == 0)
	{
		if (!KillProcess(pData->dwProcessId))
			AfxThrowOleException(HRESULT_FROM_WIN32(GetLastError()));
	}
	else
	{
		_ASSERTE(m_pfnVDMTerminateTaskWOW != NULL);

		if (!m_pfnVDMTerminateTaskWOW(pData->dwProcessId, 
									  (WORD)pData->dwWowTaskId))
			AfxThrowOleException(HRESULT_FROM_WIN32(GetLastError()));
	}

	Sleep(100);
	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnActionKillProc_Update
//
//  Updates the state of Action|End Process menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnActionKillProc_Update(
	CCmdUI * pCmdUI
	) 
{
	BOOL bEnable = FALSE;

	if (m_strMachineName.IsEmpty() && m_bProcesses)
	{
		int nSel = m_wndView.GetNextItem(-1, LVNI_SELECTED);
		bEnable = nSel != -1;
	}

	pCmdUI->Enable(bEnable);
}

//---------------------------------------------------------------------------
// OnActionKillTree
//
//  Handles Action|End Process Tree menu command.
// 
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnActionKillTree() 
{
	TCHAR szMessage[1024];
	TCHAR szTitle[256];

	AfxLoadString(IDS_KILLTREE_WARNING, szMessage, countof(szMessage));
	AfxLoadString(IDS_WARNING_TITLE, szTitle, countof(szTitle));

	if (MessageBox(szMessage, szTitle, MB_YESNO|MB_ICONEXCLAMATION) != IDYES)
		return;

	int nSel = m_wndView.GetNextItem(-1, LVNI_SELECTED);
	_ASSERTE(nSel != -1);

	CItemData * pData = (CItemData *)m_wndView.GetItemData(nSel);
	_ASSERTE(_CrtIsValidHeapPointer(pData));

	if (!KillProcessEx(pData->dwProcessId, TRUE))
	{
		Sleep(100);
		OnViewRefresh();

		AfxThrowOleException(HRESULT_FROM_WIN32(GetLastError()));
	}

	Sleep(100);
	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnActionKillTree_Update
//
//  Updates the state of Action|End Process Tree menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnActionKillTree_Update(
	CCmdUI * pCmdUI
	)
{
	BOOL bEnable = FALSE;

	if (m_strMachineName.IsEmpty() && m_bProcesses)
	{
		int nSel = m_wndView.GetNextItem(-1, LVNI_SELECTED);
		if (nSel != -1)
		{
			CItemData * pData = (CItemData *)m_wndView.GetItemData(nSel);
			bEnable = pData->dwWowTaskId == 0;
		}
	}

	pCmdUI->Enable(bEnable);
}

//---------------------------------------------------------------------------
// OnActionKillApp
//
//  Handles Action|End Application menu command.
// 
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnActionKillApp() 
{
	int nSel = m_wndView.GetNextItem(-1, LVNI_SELECTED);
	_ASSERTE(nSel != -1);

	CItemData * pData = (CItemData *)m_wndView.GetItemData(nSel);
	_ASSERTE(_CrtIsValidHeapPointer(pData));

	m_dwWaitStart = GetTickCount();
	m_strKillApp = pData->strName;

	KillApplication(pData->hWndTask, WaitCallback, (LPARAM)this);

	m_dwWaitStart = 0;
}

//---------------------------------------------------------------------------
// OnActionKillApp_Update
//
//  Updates the state of Action|End Application menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnActionKillApp_Update(
	CCmdUI * pCmdUI
	)
{
	_ASSERTE(pCmdUI != NULL);

	BOOL bEnable = FALSE;

	if (!m_bProcesses && m_dwWaitStart == 0)
	{
		int nSel = m_wndView.GetNextItem(-1, LVNI_SELECTED);
		if (nSel != -1)
		{
			CItemData * pData = (CItemData *)m_wndView.GetItemData(nSel);
			_ASSERTE(pData != NULL);

			bEnable = pData->hWndTask != m_hWnd;
		}
	}

	pCmdUI->Enable(bEnable);
}

//---------------------------------------------------------------------------
// OnActionSecurity
//
//  Handles Action|Security... menu command.
// 
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnActionSecurity() 
{
	int nSel = m_wndView.GetNextItem(-1, LVNI_SELECTED);
	_ASSERTE(nSel != -1);

	CItemData * pData = (CItemData *)m_wndView.GetItemData(nSel);
	_ASSERTE(_CrtIsValidHeapPointer(pData));

	CProcessSecInfo SecInfo;

	if (!SecInfo.EditDacl(m_hWnd, pData->dwProcessId, pData->strName))
		AfxThrowOleException(HRESULT_FROM_WIN32(GetLastError()));
}

//---------------------------------------------------------------------------
// OnActionSecurity_Update
//
//  Updates the state of Action|Security... menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnActionSecurity_Update(
	CCmdUI * pCmdUI
	)
{
	BOOL bEnable = FALSE;

	if (m_bProcesses && m_bSedAvailable && m_strMachineName.IsEmpty())
	{
		int nSel = m_wndView.GetNextItem(-1, LVNI_SELECTED);
		if (nSel != -1)
		{
			CItemData * pData = (CItemData *)m_wndView.GetItemData(nSel);
			_ASSERTE(_CrtIsValidHeapPointer(pData));

			bEnable = pData->dwProcessId != 0 && pData->dwWowTaskId == 0;
		}
	}

	pCmdUI->Enable(bEnable);
}

//---------------------------------------------------------------------------
// OnViewApplications
//
//  Handles View|Applications menu command. Switches to the Applications
//	mode.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewApplications() 
{
	if (m_bProcesses != 0)
	{
		if (::IsWindow(m_wndView.m_hWnd))
		{
			SaveViewSettings();
			m_wndView.DestroyWindow();
		}

		DWORD dwStyle;

		// create a view to occupy the client area of the frame
		dwStyle = WS_CHILD|WS_VISIBLE|LVS_REPORT|LVS_SHAREIMAGELISTS|
				  LVS_SHOWSELALWAYS|LVS_SINGLESEL;

		RECT rcClient;
		GetClientRect(&rcClient);

		m_wndView.Create(dwStyle, rcClient, this, AFX_IDW_PANE_FIRST);
		m_wndView.ModifyStyleEx(0, WS_EX_CLIENTEDGE);
		m_wndView.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_HEADERDRAGDROP);

		m_wndView.SetImageList(&m_ImageList, LVSIL_SMALL);

		RecalcLayout();

		m_strMachineName.Empty();

		// remove any colums currently in the list
		while (m_wndView.DeleteColumn(0));

		TCHAR szColumn[256];

		// add two colums to the list control
		AfxLoadString(IDS_APPS_TASK, szColumn, countof(szColumn));
		m_wndView.InsertColumn(0, szColumn);

		AfxLoadString(IDS_APPS_STATUS, szColumn, countof(szColumn));
		m_wndView.InsertColumn(1, szColumn);

		m_bProcesses = 0;

		m_wndView.SetRedraw(FALSE);
		m_wndView.SetColumnWidth(0, m_nAppsColWidth[0]);
		m_wndView.SetColumnWidth(1, m_nAppsColWidth[1]);
		m_wndView.SetColumnOrderArray(2, m_nAppsColOrder);

		OnViewRefresh();
	}
}

//---------------------------------------------------------------------------
// OnViewApplications_Update
//
//  Updates the state of View|Applications menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewApplications_Update(
	CCmdUI * pCmdUI
	)
{
	_ASSERTE(pCmdUI != NULL);

	pCmdUI->SetRadio(!m_bProcesses);
}

//---------------------------------------------------------------------------
// OnViewProcesses
//
//  Handles View|Processes menu command. Switches to the Processes mode.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewProcesses() 
{
	if (m_bProcesses != 1)
	{
		if (::IsWindow(m_wndView.m_hWnd))
		{
			SaveViewSettings();
			m_wndView.DestroyWindow();
		}

		DWORD dwStyle;

		// create a view to occupy the client area of the frame
		dwStyle = WS_CHILD|WS_VISIBLE|LVS_REPORT|LVS_SHAREIMAGELISTS|
				  LVS_SHOWSELALWAYS|LVS_SINGLESEL;

		RECT rcClient;
		GetClientRect(&rcClient);

		m_wndView.Create(dwStyle, rcClient, this, AFX_IDW_PANE_FIRST);
		m_wndView.ModifyStyleEx(0, WS_EX_CLIENTEDGE);
		m_wndView.SetExtendedStyle(LVS_EX_FULLROWSELECT|LVS_EX_HEADERDRAGDROP);
		
		RecalcLayout();

		TCHAR szColumn[256];

		// add two colums to the list control
		AfxLoadString(IDS_PROCESSES_ID, szColumn, countof(szColumn));
		m_wndView.InsertColumn(0, szColumn);

		AfxLoadString(IDS_PROCESSES_NAME, szColumn, countof(szColumn));
		m_wndView.InsertColumn(1, szColumn);

		m_bProcesses = 1;

		m_wndView.SetRedraw(FALSE);
		m_wndView.SetColumnWidth(0, m_nProcColWidth[0]);
		m_wndView.SetColumnWidth(1, m_nProcColWidth[1]);
		m_wndView.SetColumnOrderArray(2, m_nProcColOrder);

		OnViewRefresh();

	}
}

//---------------------------------------------------------------------------
// OnViewProcesses_Update
//
//  Updates the state of View|Processes menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void CMainFrame::OnViewProcesses_Update(
	CCmdUI * pCmdUI
	) 
{
	_ASSERTE(pCmdUI != NULL);

	pCmdUI->SetRadio(m_bProcesses);
}

//---------------------------------------------------------------------------
// OnView16bit
//
//  Handles View|Show 16-bit Tasks menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void 
CMainFrame::OnView16bit() 
{
	m_bShow16Bit = !m_bShow16Bit;
	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnView16bit_Update
//
//  Updates the state of View|Show 16-bit Tasks menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnView16bit_Update(
	CCmdUI * pCmdUI
	)
{
	_ASSERTE(pCmdUI != NULL);

	BOOL bEnable = m_osvi.dwPlatformId == VER_PLATFORM_WIN32_NT &&
				   m_strMachineName.IsEmpty() &&
				   m_pfnVDMEnumTaskWOWEx != NULL;

	pCmdUI->SetCheck(m_bShow16Bit);
	pCmdUI->Enable(bEnable);
}

//---------------------------------------------------------------------------
// OnViewRefresh
//
//  Handles View|Refresh menu command. Refreshes the currently displayed
//  information.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewRefresh() 
{
	if (m_nRefreshPeriod == UPDATE_PERIOD_PAUSED)
		AfxGetApp()->BeginWaitCursor();

	m_wndView.SetRedraw(FALSE);

	// mark all items
	int nCount = m_wndView.GetItemCount();
	for (int i = 0; i < nCount; i++)
	{
		CItemData * pData = (CItemData *)m_wndView.GetItemData(i);
		_ASSERTE(pData != NULL);

		pData->bDelete = TRUE;
	}

	try
	{
		if (m_bProcesses)
			ListProcesses();
		else
			ListApplications();
	}
	catch (CException * pe)
	{
		if (m_nRefreshPeriod != UPDATE_PERIOD_PAUSED)
		{
			KillTimer(1);
			m_nRefreshPeriod = UPDATE_PERIOD_PAUSED;
		}

		pe->ReportError();
		pe->Delete();
	}

	// walk through the list and delete items which are still marked
	nCount = m_wndView.GetItemCount();
	for (i = nCount - 1; i >= 0; i--)
	{
		CItemData * pData = (CItemData *)m_wndView.GetItemData(i);
		_ASSERTE(pData != NULL);

		if (pData->bDelete)
			m_wndView.DeleteItem(i);
	}

	m_wndView.SetRedraw(TRUE);

	TCHAR szFormat[256];
	TCHAR szTitle[256];

	if (m_strMachineName.IsEmpty())
		AfxLoadString(IDS_TITLE_LOCAL, szFormat, countof(szFormat));
	else
		AfxLoadString(IDS_TITLE_REMOTE, szFormat, countof(szFormat));

	wsprintf(szTitle, szFormat, (LPCTSTR)m_strMachineName);
	SetWindowText(szTitle);

	if (m_nRefreshPeriod == UPDATE_PERIOD_PAUSED)
		AfxGetApp()->EndWaitCursor();
}

//---------------------------------------------------------------------------
// OnViewSpeedHigh
//
//  Handles View|Update Speed|High menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewSpeedHigh() 
{
	if (m_nRefreshPeriod != UPDATE_PERIOD_HIGH)
	{
		m_nRefreshPeriod = UPDATE_PERIOD_HIGH;
		SetTimer(1, UPDATE_PERIOD_HIGH, NULL);
	}
}

//---------------------------------------------------------------------------
// OnViewSpeedHigh_Update
//
//  Updates the state of View|Update Speed|High menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewSpeedHigh_Update(
	CCmdUI * pCmdUI
	)
{
	_ASSERTE(pCmdUI != NULL);

	pCmdUI->SetRadio(m_nRefreshPeriod == UPDATE_PERIOD_HIGH);
	pCmdUI->Enable(m_strMachineName.IsEmpty());
}

//---------------------------------------------------------------------------
// OnViewSpeedNormal
//
//  Handles View|Update Speed|Normal menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewSpeedNormal() 
{
	if (m_nRefreshPeriod != UPDATE_PERIOD_NORMAL)
	{
		m_nRefreshPeriod = UPDATE_PERIOD_NORMAL;
		SetTimer(1, UPDATE_PERIOD_NORMAL, NULL);
	}
}

//---------------------------------------------------------------------------
// OnViewSpeedNormal_Update
//
//  Updates the state of View|Update Speed|Normal menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewSpeedNormal_Update(
	CCmdUI * pCmdUI
	)
{
	_ASSERTE(pCmdUI != NULL);

	pCmdUI->SetRadio(m_nRefreshPeriod == UPDATE_PERIOD_NORMAL);
	pCmdUI->Enable(m_strMachineName.IsEmpty());
}

//---------------------------------------------------------------------------
// OnViewSpeedLow
//
//  Handles View|Update Speed|Low menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewSpeedLow() 
{
	if (m_nRefreshPeriod != UPDATE_PERIOD_LOW)
	{
		m_nRefreshPeriod = UPDATE_PERIOD_LOW;
		SetTimer(1, UPDATE_PERIOD_LOW, NULL);
	}
}

//---------------------------------------------------------------------------
// OnViewSpeedLow_Update
//
//  Updates the state of View|Update Speed|Low menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewSpeedLow_Update(
	CCmdUI * pCmdUI
	)
{
	_ASSERTE(pCmdUI != NULL);

	pCmdUI->SetRadio(m_nRefreshPeriod == UPDATE_PERIOD_LOW);
	pCmdUI->Enable(m_strMachineName.IsEmpty());
}

//---------------------------------------------------------------------------
// OnViewSpeedPaused
//
//  Handles View|Update Speed|Paused menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewSpeedPaused() 
{
	if (m_nRefreshPeriod != UPDATE_PERIOD_PAUSED)
	{
		m_nRefreshPeriod = UPDATE_PERIOD_PAUSED;
		KillTimer(1);
	}
}

//---------------------------------------------------------------------------
// OnViewSpeedPaused_Update
//
//  Updates the state of View|Update Speed|Paused menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnViewSpeedPaused_Update(
	CCmdUI * pCmdUI
	)
{
	_ASSERTE(pCmdUI != NULL);

	pCmdUI->SetRadio(m_nRefreshPeriod == UPDATE_PERIOD_PAUSED);
}

//---------------------------------------------------------------------------
// OnOptionsEnumprocPsapi
//
//  Handles Options|Enumerate Processes With|PSAPI menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnOptionsEnumprocPsapi()
{
	m_nEnumProcMethod = ENUMPROCESSES_PSAPI;
	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnOptionsEnumprocPsapi_Update
//
//  Updates the state of Options|Enumerate Processes With|PSAPI menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void 
CMainFrame::OnOptionsEnumprocPsapi_Update(
	CCmdUI * pCmdUI
	)
{
	_ASSERTE(pCmdUI != NULL);

	pCmdUI->Enable(m_hPSAPI != NULL && m_strMachineName.IsEmpty());
	pCmdUI->SetRadio(m_nEnumProcMethod == ENUMPROCESSES_PSAPI);
}

//---------------------------------------------------------------------------
// OnOptionsEnumprocToolhelp
//
//  Handles Options|Enumerate Processes With|ToolHelp32 API menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void 
CMainFrame::OnOptionsEnumprocToolhelp() 
{
	m_nEnumProcMethod = ENUMPROCESSES_TOOLHELP;
	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnOptionsEnumprocToolhelp_Update
//
//  Updates the state of Options|Enumerate Processes With|ToolHelp32 API menu
//  command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void CMainFrame::OnOptionsEnumprocToolhelp_Update(CCmdUI* pCmdUI) 
{
	_ASSERTE(pCmdUI != NULL);

	BOOL bEnable = (m_osvi.dwPlatformId == VER_PLATFORM_WIN32_NT &&
					m_osvi.dwMajorVersion >= 5) ||
				   m_osvi.dwPlatformId == VER_PLATFORM_WIN32_WINDOWS;

	pCmdUI->Enable(bEnable && m_strMachineName.IsEmpty());
	pCmdUI->SetRadio(m_nEnumProcMethod == ENUMPROCESSES_TOOLHELP);
}

//---------------------------------------------------------------------------
// OnOptionsEnumprocNtapi
//
//  Handles Options|Enumerate Processes With|ZwQuerySystemInformation menu
//  command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void CMainFrame::OnOptionsEnumprocNtapi() 
{
	m_nEnumProcMethod = ENUMPROCESSES_NTAPI;
	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnOptionsEnumprocNtapi_Update
//
//  Updates the state of Options|Enumerate Processes With|ZwQuerySystemInfor-
//	mation menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnOptionsEnumprocNtapi_Update(
	CCmdUI * pCmdUI
	) 
{
	_ASSERTE(pCmdUI != NULL);

	pCmdUI->Enable(m_osvi.dwPlatformId == VER_PLATFORM_WIN32_NT &&
				   m_strMachineName.IsEmpty());
	pCmdUI->SetRadio(m_nEnumProcMethod == ENUMPROCESSES_NTAPI);
}

//---------------------------------------------------------------------------
// OnOptionsEnumprocPerfdata
//
//  Handles Options|Enumerate Processes With|Performance Data menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void CMainFrame::OnOptionsEnumprocPerfdata() 
{
	m_nEnumProcMethod = ENUMPROCESSES_PERFDATA;
	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnOptionsEnumprocPerfdata_Update
//
//  Updates the state of Options|Enumerate Processes With|Performance Data
//  menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnOptionsEnumprocPerfdata_Update(
	CCmdUI * pCmdUI
	) 
{
	_ASSERTE(pCmdUI != NULL);

	pCmdUI->Enable(m_hPDH != NULL);
	pCmdUI->SetRadio(m_nEnumProcMethod == ENUMPROCESSES_PERFDATA);
}

//---------------------------------------------------------------------------
// OnOptionsEnumprocWmi
//
//  Handles Options|Enumerate Processes With|Windows Management Instrumenta-
//	tion menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnOptionsEnumprocWmi() 
{
	m_nEnumProcMethod = ENUMPROCESSES_WMI;
	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnOptionsEnumprocWmi_Update
//
//  Updates the state of Options|Enumerate Processes With|Windows Management
//  Instrumentation menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnOptionsEnumprocWmi_Update(
	CCmdUI * pCmdUI
	) 
{
	_ASSERTE(pCmdUI != NULL);

	pCmdUI->Enable(m_bWmiAvailable != NULL);
	pCmdUI->SetRadio(m_nEnumProcMethod == ENUMPROCESSES_WMI);	
}

//---------------------------------------------------------------------------
// OnOptionsHungupSmto
//
//  Handles Options|Detect Hangup With|SendMessageTimeout menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnOptionsHangupSmto() 
{
	m_pfnIsAppHung = IsAppHung_SMTO;
	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnOptionsHungupSmto_Update
//
//  Updates the state of Options|Detect Hangup With|SendMessageTimeout menu
//  command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnOptionsHangupSmto_Update(
	CCmdUI * pCmdUI
	)
{
	_ASSERTE(pCmdUI != NULL);
	
	pCmdUI->SetRadio(m_pfnIsAppHung == IsAppHung_SMTO);
}

//---------------------------------------------------------------------------
// OnOptionsHungupUndoc
//
//  Handles Options|Detect Hangup With|IsHungAppWindow/IsHungThread menu
//  command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnOptionsHangupUndoc() 
{
	m_pfnIsAppHung = IsAppHung_Undoc;
	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnOptionsHungupUndoc_Update
//
//  Updates the state of Options|Detect Hangup With|IsHungAppWindow/
//  IsHungThread menu command.
//
//  Parameters:
//	  pCmdUI - pointer to the command update object
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnOptionsHangupUndoc_Update(
	CCmdUI * pCmdUI
	)
{
	_ASSERTE(pCmdUI != NULL);
	
	pCmdUI->SetRadio(m_pfnIsAppHung == IsAppHung_Undoc);
}

//---------------------------------------------------------------------------
// OnOptionsDebug
//
//  Handles Options|Enable Debug Privilege menu command.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnOptionsDebug() 
{
	TOKEN_PRIVILEGES tkp;
	tkp.PrivilegeCount = 1;

	LookupPrivilegeValue(NULL, SE_DEBUG_NAME, &tkp.Privileges[0].Luid);

	BOOL bEnabled;
	_VERIFY(IsPrivilegeEnabled(&tkp.Privileges[0].Luid, &bEnabled));

	if (bEnabled)
		tkp.Privileges[0].Attributes = 0;
	else
		tkp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;


	HANDLE hToken;
	if (OpenProcessToken(GetCurrentProcess(), 
						 TOKEN_QUERY|TOKEN_ADJUST_PRIVILEGES,
						 &hToken))
	{
		AdjustTokenPrivileges(hToken, FALSE, &tkp, sizeof(tkp), NULL, NULL);
		CloseHandle(hToken);
	}

	OnViewRefresh();
}

//---------------------------------------------------------------------------
// OnOptionsDebug_Update
//
//  Updates the state of Options|Enable Debug Privilege menu command.
//
//  Paramters:
//	  pCmdUI - pointer to the command update object
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnOptionsDebug_Update(
	CCmdUI* pCmdUI
	) 
{
	_ASSERTE(pCmdUI != NULL);

	LUID Luid;
	LookupPrivilegeValue(NULL, SE_DEBUG_NAME, &Luid);

	if (m_osvi.dwPlatformId == VER_PLATFORM_WIN32_NT)
	{
		BOOL bEnabled;
		if (IsPrivilegeEnabled(&Luid, &bEnabled))
			pCmdUI->SetCheck(bEnabled);
		else
			pCmdUI->Enable(FALSE);
	}
	else
		pCmdUI->Enable(FALSE);
}

//---------------------------------------------------------------------------
// OnList_ColumnClick
//
//  Called when the list column header has been clicked.
//
//  Parameters:
//	  pNMHDR  - notification parameters
//	  pResult - pointer to a variable that receives notification result
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnList_ColumnClick(
	NMHDR * pNMHDR,
	LRESULT * pResult
	)
{
	_UNUSED(pResult);

	_ASSERTE(pNMHDR != NULL);
	_ASSERTE(pResult != NULL);

	NMLISTVIEW * pnmlv = (NMLISTVIEW *)pNMHDR;

	if (m_bProcesses)
	{
		if (pnmlv->iSubItem == abs(m_nSortOrder) - 1)
			m_nSortOrder = -m_nSortOrder;
		else
			m_nSortOrder = -(pnmlv->iSubItem + 1);

		m_wndView.SortItems(SortCallback, m_nSortOrder);
		SetSortMark(m_nSortOrder);
	}
	else
	{
		if (pnmlv->iSubItem == abs(m_nAppsSortOrder) - 1)
			m_nAppsSortOrder = -m_nAppsSortOrder;
		else
			m_nAppsSortOrder = -(pnmlv->iSubItem + 1);

		m_wndView.SortItems(AppsSortCallback, m_nAppsSortOrder);
		SetSortMark(m_nAppsSortOrder);
	}
}

//---------------------------------------------------------------------------
// OnList_DeleteItem
//
//  Called when an item is about to be deleted from the list.
//
//  Parameters:
//	  pNMHDR  - notification parameters
//	  pResult - pointer to a variable that receives notification result
//
//  Returns:
//	  no return value.
//
void
CMainFrame::OnList_DeleteItem(
	NMHDR * pNMHDR,
	LRESULT * pResult
	)
{
	_UNUSED(pResult);

	_ASSERTE(pNMHDR != NULL);
	_ASSERTE(pResult != NULL);

	NMLISTVIEW * pnmlv = (NMLISTVIEW *)pNMHDR;
	CItemData * pData = (CItemData *)m_wndView.GetItemData(pnmlv->iItem);

	_ASSERTE(pData != NULL);

	delete pData;
}

//---------------------------------------------------------------------------
// SaveViewSettings
//
//  Saves current view settings in the member variables.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::SaveViewSettings()
{
	if (m_bProcesses)
	{
		m_nProcColWidth[0] = m_wndView.GetColumnWidth(0);
		m_nProcColWidth[1] = m_wndView.GetColumnWidth(1);
		m_wndView.GetColumnOrderArray(m_nProcColOrder, 2);
	}
	else
	{
		m_nAppsColWidth[0] = m_wndView.GetColumnWidth(0);
		m_nAppsColWidth[1] = m_wndView.GetColumnWidth(1);
		m_wndView.GetColumnOrderArray(m_nAppsColOrder, 2);
	}
}

//---------------------------------------------------------------------------
// SetSortMark
//
//  Sets the sort mark in the list control.
//
//  Parameters:
//	  nOrder - sort order
//
//  Returns:
//	  no return value.
//
void 
CMainFrame::SetSortMark(
	int nOrder
	)
{
	int nSubItem = abs(nOrder) - 1;	

	CHeaderCtrl& hdr = *m_wndView.GetHeaderCtrl();

	int nCount = hdr.GetItemCount();
	for (int i = 0; i < nCount; i++)
	{
		HDITEM hdi;
		hdi.mask = HDI_FORMAT;

		_VERIFY(hdr.GetItem(i, &hdi));

		if (i != nSubItem)
		{
			hdi.fmt &= ~(HDF_BITMAP|HDF_BITMAP_ON_RIGHT);
		}
		else
		{
			hdi.mask |= HDI_BITMAP;
			hdi.fmt |= HDF_BITMAP|HDF_BITMAP_ON_RIGHT;
			hdi.hbm = (nOrder > 0) ? m_bmSortDown : m_bmSortUp;
		}

		_VERIFY(hdr.SetItem(i, &hdi));
	}
}

//---------------------------------------------------------------------------
// SortCallback
//
//  This function is called to compare list view items during the sort
//	process.
//
//  Parameters:
//	  lParam1    - data associated with the first item
//	  lParam2    - data associated with the second item
//	  lParamSort - contains a pointer to the CSortData structure.
//
//  Returns:
//	  less than zero, if the first item is less than the second;
//	  greater than zero, if the first item is greater than the second;
//	  zero, if the items are equal.
//
int
CALLBACK
CMainFrame::SortCallback(
	LPARAM lParam1,
	LPARAM lParam2,
	LPARAM lParamSort
	)
{
	CItemData * pData1 = (CItemData *)lParam1;
	CItemData * pData2 = (CItemData *)lParam2;

	_ASSERTE(_CrtIsValidHeapPointer(pData1));
	_ASSERTE(_CrtIsValidHeapPointer(pData2));

	int nRes;

	switch (abs(lParamSort))
	{
		// sort on process identifiers
		case 1:
			nRes = (int)pData2->dwProcessId - (int)pData1->dwProcessId;
			break;

		// sort on process names
		case 2:
			nRes = lstrcmpi(pData2->strName, pData1->strName);
			break;

		default:
			_ASSERTE(0);
			__assume(0);
	}

	if (lParamSort < 0)
		nRes = -nRes;

	if (nRes == 0)
		nRes = (int)pData1->dwWowTaskId - (int)pData2->dwWowTaskId;

	return nRes;
}

//---------------------------------------------------------------------------
// AppsSortCallback
//
//  This function is called to compare list view items during the sort
//	process.
//
//  Parameters:
//	  lParam1    - data associated with the first item
//	  lParam2    - data associated with the second item
//	  lParamSort - contains a pointer to the CSortData structure.
//
//  Returns:
//	  less than zero, if the first item is less than the second;
//	  greater than zero, if the first item is greater than the second;
//	  zero, if the items are equal.
//
int
CALLBACK
CMainFrame::AppsSortCallback(
	LPARAM lParam1,
	LPARAM lParam2,
	LPARAM lParamSort
	)
{
	CItemData * pData1 = (CItemData *)lParam1;
	CItemData * pData2 = (CItemData *)lParam2;

	_ASSERTE(_CrtIsValidHeapPointer(pData1));
	_ASSERTE(_CrtIsValidHeapPointer(pData2));

	int nRes;

	switch (abs(lParamSort))
	{
		// sort on application names
		case 1:
			nRes = lstrcmpi(pData2->strName, pData1->strName);
			break;

		// sort on application status
		case 2:
			if (pData1->bRunning && !pData2->bRunning)
				nRes = 1;
			else if (!pData1->bRunning && pData2->bRunning)
				nRes = -1;
			else
				nRes = lstrcmpi(pData2->strName, pData1->strName);
			break;

		default:
			_ASSERTE(0);
			__assume(0);
	}

	if (lParamSort < 0)
		nRes = -nRes;

	return nRes;
}

//---------------------------------------------------------------------------
// ListProcesses
//
//  Fills the list control with the list of processes.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::ListProcesses()
{
	PFNENUMPROCESSES pfnEnumProc = NULL;
	
	// select enumeration function to use
	switch (m_nEnumProcMethod)
	{
		case ENUMPROCESSES_PSAPI:
			pfnEnumProc = EnumProcesses_PsApi;
			break;
		case ENUMPROCESSES_TOOLHELP:
			pfnEnumProc = EnumProcesses_ToolHelp;
			break;
		case ENUMPROCESSES_NTAPI:
			pfnEnumProc = EnumProcesses_NtApi;
			break;
		case ENUMPROCESSES_PERFDATA:
			pfnEnumProc = EnumProcesses_PerfData;
			break;
		case ENUMPROCESSES_WMI:
			pfnEnumProc = EnumProcesses_Wmi;
			break;
		default:
			_ASSERTE(0);
			__assume(0);
	}

	_ASSERTE(pfnEnumProc != NULL);

	LPCTSTR pszMachineName = NULL;
	if (!m_strMachineName.IsEmpty())
		pszMachineName = m_strMachineName;

	// call process enumeration function
	if (!pfnEnumProc(pszMachineName, EnumProcessCallback, (LPARAM)this))
		AfxThrowOleException(HRESULT_FROM_WIN32(GetLastError()));

	m_wndView.SortItems(SortCallback, m_nSortOrder);
	SetSortMark(m_nSortOrder);
}

//---------------------------------------------------------------------------
// ListApplications
//
//  Fills the list control with the list of applications.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CMainFrame::ListApplications()
{
	int nCount = m_ImageList.GetImageCount();
	for (int i = nCount - 1; i > 0; i--)
		m_ImageList.Remove(i);

	if (!EnumApplications(EnumApplicationCallback, (LPARAM)this))
		AfxThrowOleException(HRESULT_FROM_WIN32(GetLastError()));

	m_wndView.SortItems(AppsSortCallback, m_nAppsSortOrder);
	SetSortMark(m_nAppsSortOrder);
}

//---------------------------------------------------------------------------
// EnumProcessCallback
//
//  This function is called during processes enumeration. It adds an item
//  into the list control that corresponds to the specified process.
//
//  Parameters:
//	  dwProcessId - process identifier
//	  pszName	  - name of the process; this parameter can be NULL if the
//					process name is not available
//	  lParam	  - contains address of the CMainFrame object
//
//  Returns:
//	  TRUE, to continue enumeration; FALSE, to stop enumeration.
//
BOOL
CALLBACK
CMainFrame::EnumProcessCallback(
	DWORD dwProcessId,
	LPCTSTR pszName,
	LPARAM lParam
	)
{
	CMainFrame * pFrame = (CMainFrame *)lParam;
	ASSERT_VALID(pFrame);

	CListCtrl& wndView = pFrame->m_wndView;
	CItemData * pData;
	BOOL bFound = FALSE;

	TCHAR szID[64];
	wsprintf(szID, _T("%u (0x%X)"), dwProcessId, dwProcessId);

	TCHAR szUnavailable[256];
	if (pszName == NULL)
	{
		AfxLoadString(IDS_NAME_UNAVAILABLE, szUnavailable, 
						countof(szUnavailable));
		pszName = szUnavailable;
	}

	// try to find the corresponding item in the list
	int nCount = wndView.GetItemCount();
	for (int nItem = 0; nItem < nCount; nItem++)
	{
		pData = (CItemData *)wndView.GetItemData(nItem);
		if (pData->dwProcessId == dwProcessId &&
			pData->dwWowTaskId == 0 &&
			pData->strName == pszName)
		{
			pData->bDelete = FALSE;
			bFound = TRUE;
			break;
		}
	}

	if (!bFound)
	{
		pData = new CItemData(dwProcessId, DWORD(0));

		nItem = wndView.InsertItem(LVIF_TEXT|LVIF_PARAM, nCount, szID, 
								   0, 0, -1, (LPARAM)pData);
		if (nItem == -1)
		{
			delete pData;
			return TRUE;
		}

		pData->strName = pszName;
		wndView.SetItemText(nItem, 1, pszName);
	}

	if (pFrame->m_bShow16Bit &&
		(lstrcmpi(pszName, _T("NTVDM.EXE")) == 0 ||
		 lstrcmpi(pszName, _T("NTVDM")) == 0))
	{
		_ASSERTE(pFrame->m_pfnVDMEnumTaskWOWEx != NULL);

		pFrame->m_dwProcessId = dwProcessId;
		pFrame->m_pfnVDMEnumTaskWOWEx(dwProcessId, EnumTask16Callback,
									  (LPARAM)pFrame);
	}

	return TRUE;
}

//---------------------------------------------------------------------------
// EnumTask16Callback
//
//  This function is called during enumeration of 16-bit tasks.
//
//  Parameters:
//	  dwThreadId  - thread identifier
//	  hMod16	  - 16-bit module identifier
//	  hTask16	  - 16-bit task identifier
//	  pszModName  - module name
//	  pszFileName - file name
//	  lParam	  - contains a pointer to the CMainFrame object
//
//  Returns:
//	  FALSE to continue enumeration, TRUE to stop enumeration.
//
BOOL
CALLBACK
CMainFrame::EnumTask16Callback(
	DWORD dwThreadId,
	WORD hMod16,
	WORD hTask16,
	LPSTR pszModName,
	LPSTR pszFileName,
	LPARAM lParam
	)
{
	_UNUSED(dwThreadId);
	_UNUSED(hMod16);
	_UNUSED(pszModName);

	CMainFrame * pFrame = (CMainFrame *)lParam;
	ASSERT_VALID(pFrame);

	CListCtrl& wndView = pFrame->m_wndView;
	DWORD dwProcessId = pFrame->m_dwProcessId;
	CItemData * pData;

	USES_CONVERSION;
	LPCTSTR pszFileNameT = A2CT(pszFileName);

	int nCount = wndView.GetItemCount();
	for (int nItem = 0; nItem < nCount; nItem++)
	{
		pData = (CItemData *)wndView.GetItemData(nItem);
		_ASSERTE(pData != NULL);

		if (pData->dwProcessId == dwProcessId &&
			pData->dwWowTaskId == (DWORD)hTask16 &&
			lstrcmpi(((LPCTSTR)pData->strWowName) + 2, pszFileNameT) == 0)
		{
			pData->bDelete = FALSE;
			return FALSE;
		}
	}

	// look for parent process, it has to be in the list
	for (nItem = 0; nItem < nCount; nItem++)
	{
		pData = (CItemData *)wndView.GetItemData(nItem);
		_ASSERTE(pData != NULL);

		if (pData->dwProcessId == dwProcessId &&
			pData->dwWowTaskId == 0)
			break;
	}
	_ASSERTE(nItem < nCount);

	CString& strName = pData->strName;

	pData = new CItemData(dwProcessId, hTask16);
	pData->strName = strName;

	nItem = wndView.InsertItem(LVIF_TEXT|LVIF_PARAM, nCount, _T(""), 
							   0, 0, -1, (LPARAM)pData);

	if (nItem == -1)
	{
		delete pData;
		return FALSE;
	}

	if (pszFileName != NULL)
	{
		USES_CONVERSION;

		TCHAR szFileName[MAX_PATH];

		lstrcpy(szFileName, _T("  "));
		_tcsncat(szFileName, pszFileNameT, MAX_PATH - 2);
		szFileName[MAX_PATH - 1] = 0;

		pData->strWowName = szFileName;
		wndView.SetItemText(nItem, 1, szFileName);
	}

	return FALSE;
}

//---------------------------------------------------------------------------
// EnumApplicationCallback
//
//  This function is called for each application during enumeration.
//
//  Parameters:
//	  dwProcessId - process identifier
//	  dwThreadId  - thread identifier
//	  hWnd		  - window handle
//	  pszName	  - application name
//	  lParam	  - contains a pointer to CMainFrame object
//
//  Returns:
//	  TRUE, to continue enumeration, FALSE - to stop enumeration.
//
BOOL
CALLBACK
CMainFrame::EnumApplicationCallback(
	HWND hWnd,
	LPCTSTR pszName,
	HICON hIcon,
	LPARAM lParam
	)
{
	CMainFrame * pFrame = (CMainFrame *)lParam;
	ASSERT_VALID(pFrame);

	DWORD dwProcessId;
	DWORD dwThreadId = GetWindowThreadProcessId(hWnd, &dwProcessId);
	
	CListCtrl& wndView = pFrame->m_wndView;
	CItemData * pData;
	BOOL bFound = FALSE;

	int nCount = wndView.GetItemCount();
	for (int nItem = 0; nItem < nCount; nItem++)
	{
		pData = (CItemData *)wndView.GetItemData(nItem);
		_ASSERTE(pData != NULL);

		if (pData->dwProcessId == dwProcessId &&
			pData->dwThreadId == dwThreadId &&
			pData->hWndTask == hWnd)
		{
			pData->bDelete = FALSE;
			bFound = TRUE;
			break;
		}
	}

	if (!bFound)
	{
		pData = new CItemData(dwProcessId, hWnd);

		pData->dwThreadId = dwThreadId;
		pData->strName = pszName;

		nItem = wndView.InsertItem(LVIF_TEXT|LVIF_PARAM, nCount, pszName, 
								   0, 0, -1, (LPARAM)pData);
		if (nItem == -1)
		{
			delete pData;
			return TRUE;
		}
	}
	else
	{
		wndView.SetItem(nItem, 0, LVIF_TEXT, pszName, -1, 0, 0, 0);
	}

	int nImage = 0;
	if (hIcon != NULL)
		nImage = pFrame->m_ImageList.Add(hIcon);

	wndView.SetItem(nItem, 0, LVIF_IMAGE, NULL, nImage, 0, 0, 0);

	BOOL bHung;
	pFrame->m_pfnIsAppHung(hWnd, &bHung);

	UINT nStatus = bHung ? IDS_STATUS_NOT_RESPONDING : IDS_STATUS_RUNNING;
	pData->bRunning = !bHung;

	TCHAR szStatus[40];
	AfxLoadString(nStatus, szStatus, countof(szStatus));

	wndView.SetItemText(nItem, 1, szStatus);
	return TRUE;
}

//---------------------------------------------------------------------------
// WaitCallback
//
//  This function is called periodically by KillApplication to allow the
//  calling thread to process messages while it waits for application
//	termination.
//
//  Parameters:
//	  lParam - contains a pointer to CMainFram object.
//
//  Returns:
//    KILLAPP_TERMINATE	- to stop waiting and terminate the application
//						- immediately
//	  KILLAPP_WAIT		- to continue waiting
//	  KILLAPP_CANCEL	- to cancel the operation
//
int
CALLBACK
CMainFrame::WaitCallback(
	IN LPARAM lParam
	)
{
	CMainFrame * pFrame = (CMainFrame *)lParam;
	ASSERT_VALID(pFrame);

	MSG msg;
	while (PeekMessage(&msg, NULL, 0, 0, PM_NOREMOVE))
	{
		if (!AfxGetThread()->PumpMessage())
			return KILLAPP_CANCEL;
	}

	DWORD dwTime = GetTickCount();
	if (dwTime - pFrame->m_dwWaitStart > 15000)
	{
		CEndAppDlg dlg(pFrame);
		dlg.m_strAppTitle = pFrame->m_strKillApp;

		switch (dlg.DoModal())
		{
			case IDCANCEL:
				return KILLAPP_CANCEL;
			case IDRETRY:
				pFrame->m_dwWaitStart = dwTime;
				return KILLAPP_WAIT;
			case IDABORT:
				return KILLAPP_TERMINATE;
			default:
				_ASSERTE(0);
				__assume(0);
		}
	}

	return KILLAPP_WAIT;
}

//---------------------------------------------------------------------------
// IsPrivilegeEnabled
//
//  Determines whether the specified privilege is enabled in the process
//  token.
//
//  Parameters:
//	  pLuid    - privilege LUID
//    pEnabled - pointer to a variable that receives enabled state of the
//				 privilege
//
//  Returns:
//    if successful, and if the specified privilege exists in the process
//	  token, the return value is TRUE;
//	  if failed, or if the specified privilege does not exist in the process
//	  token, the return value is FALSE.
//
BOOL
CMainFrame::IsPrivilegeEnabled(
	IN PLUID pLuid,
	OUT BOOL * pEnabled
	)
{
	_ASSERTE(pLuid != NULL);
	_ASSERTE(pEnabled != NULL);

	*pEnabled = FALSE;

	HANDLE hToken;

	if (!OpenProcessToken(GetCurrentProcess(), TOKEN_QUERY, &hToken))
		return FALSE;

	PTOKEN_PRIVILEGES pPriv;
	DWORD cbPriv;

	if (!GetTokenInformation(hToken, TokenPrivileges, NULL, 0, &cbPriv))
	{
		if (GetLastError() != ERROR_INSUFFICIENT_BUFFER)
		{
			CloseHandle(hToken);
			return FALSE;
		}
	}

	pPriv = (PTOKEN_PRIVILEGES)_alloca(cbPriv);
	_ASSERTE(pPriv != NULL);

	if (!GetTokenInformation(hToken, TokenPrivileges, pPriv, cbPriv,
							 &cbPriv))
	{
		CloseHandle(hToken);
		return FALSE;
	}

	CloseHandle(hToken);

	for (ULONG i = 0; i < pPriv->PrivilegeCount; i++)
	{
		if (pPriv->Privileges[i].Luid.LowPart == pLuid->LowPart &&
			pPriv->Privileges[i].Luid.HighPart == pLuid->HighPart)
		{
			*pEnabled = (pPriv->Privileges[i].Attributes & SE_PRIVILEGE_ENABLED) != 0;
			return TRUE;
		}
	}

	return FALSE;
}
