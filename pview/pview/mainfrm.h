// mainfrm.h

#ifndef __mainfrm_h_included
#define __mainfrm_h_included

#include "hungapp.h"
#include "eadialog.h"

// process enumeration methods
enum {
	ENUMPROCESSES_INVALID = -1,
	ENUMPROCESSES_PSAPI,
	ENUMPROCESSES_TOOLHELP,
	ENUMPROCESSES_NTAPI,
	ENUMPROCESSES_PERFDATA,
	ENUMPROCESSES_WMI
};

class CMainFrame : public CFrameWnd
{
  public:

	CMainFrame();
	virtual ~CMainFrame();

	//{{AFX_VIRTUAL(CMainFrame)
	virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
	//}}AFX_VIRTUAL

  protected:

	struct CItemData 
	{
		CString		strName;
		CString		strWowName;
		DWORD		dwProcessId;
		union {
			DWORD	dwWowTaskId;
			HWND	hWndTask;
		};
		DWORD		dwThreadId;
		BOOL		bRunning;
		BOOL		bDelete;

		CItemData(DWORD dwPid, DWORD dwWowTid)
			{ dwProcessId = dwPid; dwWowTaskId = dwWowTid; bDelete = FALSE; }
		CItemData(DWORD dwPid, HWND hWnd)
			{ dwProcessId = dwPid; hWndTask = hWnd; bDelete = FALSE; }
	};

	CStatusBar					m_wndStatusBar;
	CListCtrl					m_wndView;
	CImageList					m_ImageList;
	CBitmap						m_bmSortDown;
	CBitmap						m_bmSortUp;
	OSVERSIONINFO				m_osvi;
	HINSTANCE					m_hVdmDbg;
	VDMENUMTASKWOWEXPROC		m_pfnVDMEnumTaskWOWEx;
	VDMTERMINATETASKINWOWPROC	m_pfnVDMTerminateTaskWOW;
	int							m_nSortOrder;
	int							m_nAppsSortOrder;
	int							m_bProcesses;
	BOOL						m_bShow16Bit;
	HINSTANCE					m_hPDH;
	HINSTANCE					m_hPSAPI;
	BOOL						m_bWmiAvailable;
	BOOL						m_bSedAvailable;
	int							m_nEnumProcMethod;
	CString						m_strMachineName;
	UINT						m_nRefreshPeriod;
	PFNISAPPHUNG				m_pfnIsAppHung;
	DWORD						m_dwWaitStart;
	DWORD						m_dwProcessId;
	CString						m_strKillApp;
	int							m_nProcColWidth[2];
	int							m_nAppsColWidth[2];
	int							m_nProcColOrder[2];
	int							m_nAppsColOrder[2];

	enum {
		UPDATE_PERIOD_HIGH = 500,
		UPDATE_PERIOD_NORMAL = 1000,
		UPDATE_PERIOD_LOW = 3000,
		UPDATE_PERIOD_PAUSED = 0
	};

  protected:

	//{{AFX_MSG(CMainFrame)
	afx_msg void OnContextMenu(CWnd* pWnd, CPoint point);
	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct);
	afx_msg void OnDestroy();
	afx_msg void OnSetFocus(CWnd *pOldWnd);
	afx_msg void OnViewRefresh();
	afx_msg void OnViewApplications();
	afx_msg void OnViewApplications_Update(CCmdUI* pCmdUI);
	afx_msg void OnViewProcesses();
	afx_msg void OnViewProcesses_Update(CCmdUI* pCmdUI);
	afx_msg void OnView16bit();
	afx_msg void OnView16bit_Update(CCmdUI* pCmdUI);
	afx_msg void OnOptionsEnumprocToolhelp();
	afx_msg void OnOptionsEnumprocToolhelp_Update(CCmdUI* pCmdUI);
	afx_msg void OnOptionsEnumprocPsapi();
	afx_msg void OnOptionsEnumprocPsapi_Update(CCmdUI* pCmdUI);
	afx_msg void OnOptionsEnumprocNtapi();
	afx_msg void OnOptionsEnumprocNtapi_Update(CCmdUI* pCmdUI);
	afx_msg void OnOptionsEnumprocPerfdata();
	afx_msg void OnOptionsEnumprocPerfdata_Update(CCmdUI* pCmdUI);
	afx_msg void OnOptionsDebug_Update(CCmdUI* pCmdUI);
	afx_msg void OnOptionsDebug();
	afx_msg void OnOptionsEnumprocWmi();
	afx_msg void OnOptionsEnumprocWmi_Update(CCmdUI* pCmdUI);
	afx_msg void OnActionConnect();
	afx_msg void OnActionConnect_Update(CCmdUI* pCmdUI);
	afx_msg void OnActionDisconnect();
	afx_msg void OnActionDisconnect_Update(CCmdUI* pCmdUI);
	afx_msg void OnActionKillProc();
	afx_msg void OnActionKillProc_Update(CCmdUI* pCmdUI);
	afx_msg void OnActionKillTree();
	afx_msg void OnActionKillTree_Update(CCmdUI* pCmdUI);
	afx_msg void OnViewSpeedHigh();
	afx_msg void OnViewSpeedHigh_Update(CCmdUI* pCmdUI);
	afx_msg void OnViewSpeedNormal();
	afx_msg void OnViewSpeedNormal_Update(CCmdUI* pCmdUI);
	afx_msg void OnViewSpeedLow();
	afx_msg void OnViewSpeedLow_Update(CCmdUI* pCmdUI);
	afx_msg void OnViewSpeedPaused();
	afx_msg void OnViewSpeedPaused_Update(CCmdUI* pCmdUI);
	afx_msg void OnTimer(UINT nIDEvent);
	afx_msg void OnActionSecurity();
	afx_msg void OnActionSecurity_Update(CCmdUI* pCmdUI);
	afx_msg void OnOptionsHangupSmto();
	afx_msg void OnOptionsHangupSmto_Update(CCmdUI* pCmdUI);
	afx_msg void OnOptionsHangupUndoc();
	afx_msg void OnOptionsHangupUndoc_Update(CCmdUI* pCmdUI);
	afx_msg void OnActionKillApp();
	afx_msg void OnActionKillApp_Update(CCmdUI* pCmdUI);
	afx_msg void OnSysColorChange();
	//}}AFX_MSG
	afx_msg void OnList_ColumnClick(NMHDR *, LRESULT *);
	afx_msg void OnList_DeleteItem(NMHDR *, LRESULT *);

	DECLARE_MESSAGE_MAP()

  protected:

	void SaveViewSettings();
	void SetSortMark(int nOrder);

	static int CALLBACK SortCallback(LPARAM, LPARAM, LPARAM);
	static int CALLBACK AppsSortCallback(LPARAM, LPARAM, LPARAM);

	void ListProcesses();
	void ListApplications();

	static BOOL CALLBACK EnumProcessCallback(DWORD, LPCTSTR, LPARAM);
	static BOOL CALLBACK EnumTask16Callback(DWORD, WORD, WORD, LPSTR,
											LPSTR, LPARAM);
	static BOOL CALLBACK EnumApplicationCallback(HWND, LPCTSTR, HICON, LPARAM);
	static int CALLBACK WaitCallback(LPARAM);


	BOOL IsPrivilegeEnabled(PLUID pLuid, BOOL * pEnabled);

};

#endif // __mainfrm_h_included
