// eadialog.cpp
//
// End application dialog.
//
// $Id: $
//

#include "stdafx.h"
#include "pview.h"
#include "eadialog.h"

BEGIN_MESSAGE_MAP(CEndAppDlg, CDialog)
	//{{AFX_MSG_MAP(CEndAppDlg)
	ON_BN_CLICKED(IDC_STOP, OnStop)
	ON_BN_CLICKED(IDC_WAIT1, OnWait)
	//}}AFX_MSG_MAP
END_MESSAGE_MAP()

//---------------------------------------------------------------------------
// CEndAppDlg
//
//  Constructor, initializes the object.
//
//  Parameters:
//	  pParent - pointer to the parent window
//
//  Returns:
//	  no return value.
//
CEndAppDlg::CEndAppDlg(
	CWnd * pParent
	)
  : CDialog(CEndAppDlg::IDD, pParent)
{
	//{{AFX_DATA_INIT(CEndAppDlg)
	m_strAppTitle = _T("");
	//}}AFX_DATA_INIT
}

//---------------------------------------------------------------------------
// DoDataExchange
//
//  DDX/DDV support.
//
//  Parameters:
//	  pDX - pointer to a data exchange object
//
//  Returns:
//	  no return value.
//
void
CEndAppDlg::DoDataExchange(
	CDataExchange * pDX
	)
{
	CDialog::DoDataExchange(pDX);

	//{{AFX_DATA_MAP(CEndAppDlg)
	DDX_Control(pDX, IDC_EXCL, m_ctlExcl);
	DDX_Text(pDX, IDC_APPTITLE, m_strAppTitle);
	//}}AFX_DATA_MAP
}

//---------------------------------------------------------------------------
// OnInitDialog
//
//  Handles the WM_INITDIALOG message. Initializes the icon on the dialog.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  always TRUE;
//
BOOL CEndAppDlg::OnInitDialog() 
{
	CDialog::OnInitDialog();
	
	m_ctlExcl.SetIcon(LoadIcon(NULL, IDI_EXCLAMATION));

	return TRUE;
}

//---------------------------------------------------------------------------
// OnStop
//
//  Handles Stop button click.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CEndAppDlg::OnStop() 
{
	EndDialog(IDABORT);
}

//---------------------------------------------------------------------------
// OnWait
//
//  Handles Wait button click.
//
//  Parameters:
//	  none.
//
//  Returns:
//	  no return value.
//
void
CEndAppDlg::OnWait() 
{
	EndDialog(IDRETRY);
}

