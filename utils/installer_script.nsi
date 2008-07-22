 
CRCCheck on

Name "ProActive Agent"
OutFile setup.exe

LicenseText "This program is Licensed under the GNU General Public License (GPL)."
LicenseData "Copying"

InstallDir $PROGRAMFILES\..\ProActive\ProActiveAgent

InstallDirRegKey HKLM SOFTWARE\ProActiveAgent "AgentDirectory"

ComponentText "This will install ProActive Agent on your computer. Select which optional components you want installed."

DirText "Choose a directory to install in to:"


Page license
Page components
Page directory
Page instfiles

Section "ProActive Agent"

WriteRegStr HKLM SOFTWARE\ProActiveAgent "AgentDirectory" "$INSTDIR"
WriteRegStr HKLM SOFTWARE\ProActiveAgent "ConfigLocation" "$INSTDIR\PAAgent-config.xml"

; write the uninstall keys for Windows
WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent" "DisplayName" "ProActive Agent (remove only)"
WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent" "UninstallString" '"$INSTDIR\Uninstall.exe"'

SetOutPath $INSTDIR

;write uninstaller utility
WriteUninstaller uninstall.exe

;write files

File "agentservice.bat"
File "config.xsd"
File "Copying"

File "ConfigParser.exe"
File "ConfigParser.exe.config"
File "ConfigParser.pdb"
File "ProActiveAgent.exe"
File "ProActiveAgent.pdb"
File "install.bat"
File "PAAgent-config.xml"
File "pkill.dll"
File "uninstall.bat"

File "AgentFirstSetup.exe"
File "AgentFirstSetup.pdb"

ExecWait "$INSTDIR\AgentFirstSetup.exe"

ExecWait "$INSTDIR\install.bat"

SectionEnd

Section "ProActive Agent Control"

SetOutPath $INSTDIR

File "AgentForAgent.exe"
File "AgentForAgent.pdb"


SectionEnd

Section "ProActive ScreenSaver"

SetOutPath $SYSDIR

File "ProActiveSSaver.scr"

SectionEnd


Section "Desktop shortcuts"

SetShellVarContext all ; All users
IfFileExists $INSTDIR\AgentForAgent.exe 0 +2
  CreateShortCut "$DESKTOP\AgentControl.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\AgentForAgent.exe" 0
SetShellVarContext current ; Current User

SectionEnd

Section "Start Menu Shortcuts"

SetShellVarContext all ; All users
CreateDirectory "$SMPROGRAMS\ProActiveAgent"
CreateShortCut  "$SMPROGRAMS\ProActiveAgent\AgentControl.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\AgentForAgent.exe" 0
CreateShortCut  "$SMPROGRAMS\ProActiveAgent\Uninstall ProActive Agent.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
SetShellVarContext current ; reset to current user

SectionEnd



;uninstall section

UninstallText "This will uninstall ProActive Agent. Hit next to continue."

Section "Uninstall"

	MessageBox MB_OKCANCEL "This will delete $INSTDIR and all subdirectories and files?" IDOK DoUninstall
	
	Abort "Quiting the uninstall process"
	
	DoUnInstall:
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent"
	DeleteRegKey HKLM SOFTWARE\ProActiveAgent
	
	SetShellVarContext all ; All users
	Delete "$DESKTOP\AgentControl.lnk"
	RMDir /r "$SMPROGRAMS\ProActiveAgent"
	SetShellVarContext current ; reset to current user

	    
	SetOutPath $INSTDIR
	ExecWait '"$INSTDIR\uninstall.bat"'

	RMDir /r "$INSTDIR"
	
	Delete $SYSDIR\ProActiveSSaver.scr

SectionEnd