// eadialog.h
//
// End Application dialog.
//
// $Id: $
//

#ifndef __eadialog_h_included
#define __eadialog_h_included

class CEndAppDlg : public CDialog
{
  public:

	CEndAppDlg(CWnd * pParent = NULL);

	//{{AFX_DATA(CEndAppDlg)
	enum { IDD = IDD_ENDAPP };
	CStatic	m_ctlExcl;
	CString	m_strAppTitle;
	//}}AFX_DATA

	//{{AFX_VIRTUAL(CEndAppDlg)
	virtual void DoDataExchange(CDataExchange * pDX);
	//}}AFX_VIRTUAL

  protected:

	//{{AFX_MSG(CEndAppDlg)
	afx_msg void OnStop();
	afx_msg void OnWait();
	virtual BOOL OnInitDialog();
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

#endif // __eadialog_h_included

