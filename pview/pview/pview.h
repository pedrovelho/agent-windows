// pview.h

#ifndef __pview_h_included
#define __pview_h_included

#include "resource.h"

class CApp : public CWinApp
{
  public:

	//{{AFX_VIRTUAL(CApp)
	virtual BOOL InitInstance();
	virtual int ExitInstance();
	//}}AFX_VIRTUAL

  public:

	//{{AFX_MSG(CApp)
	//}}AFX_MSG

	//DECLARE_MESSAGE_MAP()
};

extern CApp g_theApp;

#endif // __pview_h_included


