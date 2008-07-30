; CLW file contains information for the MFC ClassWizard

[General Info]
Version=1
LastClass=CMainFrame
LastTemplate=CDialog
NewFileInclude1=#include "stdafx.h"
NewFileInclude2=#include "pview.h"
LastPage=0

ClassCount=3
Class1=CApp

ResourceCount=4
Resource1=IDD_ABOUTBOX
Resource2=IDR_POPUPS
Class2=CMainFrame
Resource3=IDD_ENDAPP
Class3=CEndAppDlg
Resource4=IDR_MAINFRAME

[CLS:CApp]
Type=0
HeaderFile=pview.h
ImplementationFile=pview.cpp
Filter=N
LastObject=CApp

[CLS:CMainFrame]
Type=0
HeaderFile=mainfrm.h
ImplementationFile=mainfrm.cpp
Filter=T
BaseClass=CFrameWnd
VirtualFilter=fWC
LastObject=CMainFrame




[MNU:IDR_MAINFRAME]
Type=1
Class=CMainFrame
Command1=ID_ACTION_CONNECT
Command2=ID_ACTION_DISCONNECT
Command3=ID_ACTION_KILLPROC
Command4=ID_ACTION_KILLTREE
Command5=ID_ACTION_KILLAPP
Command6=ID_ACTION_SECURITY
Command7=ID_APP_EXIT
Command8=ID_VIEW_STATUS_BAR
Command9=ID_VIEW_APPLICATIONS
Command10=ID_VIEW_PROCESSES
Command11=ID_VIEW_REFRESH
Command12=ID_VIEW_SPEED_HIGH
Command13=ID_VIEW_SPEED_NORMAL
Command14=ID_VIEW_SPEED_LOW
Command15=ID_VIEW_SPEED_PAUSED
Command16=ID_VIEW_16BIT
Command17=ID_OPTIONS_ENUMPROC_PSAPI
Command18=ID_OPTIONS_ENUMPROC_TOOLHELP
Command19=ID_OPTIONS_ENUMPROC_NTAPI
Command20=ID_OPTIONS_ENUMPROC_PERFDATA
Command21=ID_OPTIONS_ENUMPROC_WMI
Command22=ID_OPTIONS_HANGUP_SMTO
Command23=ID_OPTIONS_HANGUP_UNDOC
Command24=ID_OPTIONS_DEBUG
CommandCount=24

[ACL:IDR_MAINFRAME]
Type=1
Class=CMainFrame
Command1=ID_VIEW_REFRESH
CommandCount=1

[MNU:IDR_POPUPS]
Type=1
Class=?
Command1=ID_ACTION_KILLPROC
Command2=ID_ACTION_KILLTREE
Command3=ID_ACTION_SECURITY
Command4=ID_ACTION_KILLAPP
CommandCount=4

[DLG:IDD_ENDAPP]
Type=1
Class=CEndAppDlg
ControlCount=6
Control1=IDC_WAIT1,button,1342242816
Control2=IDCANCEL,button,1342242816
Control3=IDC_EXCL,static,1342177283
Control4=IDC_APPTITLE,static,1342308352
Control5=IDC_STATIC,static,1342308352
Control6=IDC_STOP,button,1342242816

[CLS:CEndAppDlg]
Type=0
HeaderFile=eadialog.h
ImplementationFile=eadialog.cpp
BaseClass=CDialog
Filter=D
VirtualFilter=dWC
LastObject=CEndAppDlg

