;-----------------------------------------------------------------------------------
; A few simple macros to handle installation on x64 architecture
;-----------------------------------------------------------------------------------
!include x64.nsh

CRCCheck on

Name "ProActive Agent"
OutFile PAAgent_Setup.exe

LicenseText "This program is Licensed under the GNU General Public License (GPL)."
LicenseData "LICENSE.txt"

;-----------------------------------------------------------------------------------
; By default the installation directory is "Program Files" however
; on x64 architecture it will be translated as "Program Files (x86)"
;-----------------------------------------------------------------------------------
InstallDir $PROGRAMFILES\ProActiveAgent

ComponentText "This will install ProActive Agent on your computer. Select which optional components you want installed."

DirText "Choose a directory to install in to:"

Page license
Page components
Page directory
Page instfiles

;.NET CHECK

;-----------------------------------------------------------------------------------
; Usage
; 1 Call SetupDotNetSectionIfNeeded from .onInit function
;   This function will check if the required version 
;   or higher version of the .NETFramework is installed.
;   If .NET is NOT installed the section which installs dotnetfx is selected.
;   If .NET is installed the section which installs dotnetfx is unselected.
 
#!define SF_USELECTED  0
#!define SF_SELECTED   1
#!define SF_SECGRP     2
#!define SF_BOLD       8
#!define SF_RO         16
#!define SF_EXPAND     32
;-----------------------------------------------------------------------------------
 
!define DOT_MAJOR 2
!define DOT_MINOR 0
 
!macro SecSelect SecId
  Push $0
  IntOp $0 ${SF_SELECTED} | ${SF_RO}
  SectionSetFlags ${SecId} $0
  SectionSetInstTypes ${SecId} 1
  Pop $0
!macroend
 
!define SelectSection '!insertmacro SecSelect'
#################################
 
!macro SecUnSelect SecId
  Push $0
  IntOp $0 ${SF_USELECTED} | ${SF_RO}
  SectionSetFlags ${SecId} $0
  SectionSetText  ${SecId} ""
  Pop $0
!macroend
 
!define UnSelectSection '!insertmacro SecUnSelect'
###################################
 
!macro SecExtract SecId
  Push $0
  IntOp $0 ${SF_USELECTED} | ${SF_RO}
  SectionSetFlags ${SecId} $0
  SectionSetInstTypes ${SecId} 2
  Pop $0
!macroend
 
!define SetSectionExtract '!insertmacro SecExtract'
###################################
 
!macro Groups GroupId
  Push $0
  SectionGetFlags ${GroupId} $0
  IntOp $0 $0 | ${SF_RO}
  IntOp $0 $0 ^ ${SF_BOLD}
  IntOp $0 $0 ^ ${SF_EXPAND}
  SectionSetFlags ${GroupId} $0
  Pop $0
!macroend
 
!define SetSectionGroup "!insertmacro Groups"
####################################
 
!macro GroupRO GroupId
  Push $0
  IntOp $0 ${SF_SECGRP} | ${SF_RO}
  SectionSetFlags ${GroupId} $0
  Pop $0
!macroend
 
!define MakeGroupReadOnly '!insertmacro GroupRO'
 

Function SetupDotNetSectionIfNeeded
 
  StrCpy $0 "0"
  StrCpy $1 "SOFTWARE\Microsoft\.NETFramework" ;registry entry to look in.
  StrCpy $2 0
 
  StartEnum:
    ;Enumerate the versions installed.
    EnumRegKey $3 HKLM "$1\policy" $2
 
    ;If we don't find any versions installed, it's not here.
    StrCmp $3 "" noDotNet notEmpty
 
    ;We found something.
    notEmpty:
      ;Find out if the RegKey starts with 'v'.  
      ;If it doesn't, goto the next key.
      StrCpy $4 $3 1 0
      StrCmp $4 "v" +1 goNext
      StrCpy $4 $3 1 1
 
      ;It starts with 'v'.  Now check to see how the installed major version
      ;relates to our required major version.
      ;If it's equal check the minor version, if it's greater, 
      ;we found a good RegKey.
      IntCmp $4 ${DOT_MAJOR} +1 goNext yesDotNetReg
      ;Check the minor version.  If it's equal or greater to our requested 
      ;version then we're good.
      StrCpy $4 $3 1 3
      IntCmp $4 ${DOT_MINOR} yesDotNetReg goNext yesDotNetReg
 
    goNext:
      ;Go to the next RegKey.
      IntOp $2 $2 + 1
      goto StartEnum
 
  yesDotNetReg:
    ;Now that we've found a good RegKey, let's make sure it's actually
    ;installed by getting the install path and checking to see if the 
    ;mscorlib.dll exists.
    EnumRegValue $2 HKLM "$1\policy\$3" 0
    ;$2 should equal whatever comes after the major and minor versions 
    ;(ie, v1.1.4322)
    StrCmp $2 "" noDotNet
    ReadRegStr $4 HKLM $1 "InstallRoot"
    ;Hopefully the install root isn't empty.
    StrCmp $4 "" noDotNet
    ;build the actuall directory path to mscorlib.dll.
    StrCpy $4 "$4$3.$2\mscorlib.dll"
    IfFileExists $4 yesDotNet noDotNet
 
  noDotNet:
    MessageBox MB_OK "You must have .NET framework >= 2.0 installed in order to use ProActive Agent."
    Abort
 
  yesDotNet:
 
FunctionEnd
; ---------------------------------------------------------------------------------

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section "ProActive Agent"

        ;-----------------------------------------------------------------------------------
        ; Check user admin rights 
        ;-----------------------------------------------------------------------------------
        System::Call "kernel32::GetModuleHandle(t 'shell32.dll') i .s"
        System::Call "kernel32::GetProcAddress(i s, i 680) i .r0"
        System::Call "::$0() i .r0"
        DetailPrint "is admin? $0"
        StrCmp $0 '0' 0 +3
          MessageBox MB_OK "Adminstrator rights are required to install the ProActive Agent."
            Abort

        ;-----------------------------------------------------------------------------------
        ; On x64 we have to explicitely set the registery view
        ;-----------------------------------------------------------------------------------
        ${If} ${RunningX64}
          SetRegView 64
        ${EndIf}
        
        ;-----------------------------------------------------------------------------------
        ; Check if User Account Protection is Activated (Windows Vista)
        ;-----------------------------------------------------------------------------------
        ReadRegDWORD $1 HKLM Software\Microsoft\Windows\CurrentVersion\Policies\System EnableLUA
        StrCmp $1 '1' 0 +3
          MessageBox MB_OK "It appears that the User Account Control (UAC) feature is enabled. The installation cannot continue. Please disable the UAC feature and restart the installation. To disable the UAC feature: \n 1) Launch MSCONFIG by from the Run menu. \n 2) Click on the Tools tab. Scroll down till you find 'Disable UAC' and reboot."
            Abort

        ; Check if .NET framework is installed >= 2.0
        DetailPrint ".NET Framework version check"
        Call SetupDotNetSectionIfNeeded
        
        ; Check if VC++ redist 2008 is installed
        ReadRegDWORD $0 HKLM Software\Microsoft\DevDiv\VC\Servicing\9.0\RED\1033 Install
        DetailPrint "VC++ 9.0 Redistributable Package status: $0"
        ; If the redistributable package is not installed run the installer
        StrCmp $0 '1' continueInstall 0
          MessageBox MB_OK 'You must install the Visual C++ 2008 Redistributable Package to use ProActive Agent.$\nPress OK to begin installation.'
          ; Prepare to copy the redistributable package
          SetOutPath $TEMP
          ; Copy the architecture dependant installer
          ${If} ${RunningX64}
          File "utils\x64\vcredist_x64_2008.exe"
          ${Else}
          File "utils\x86\vcredist_x86_2008.exe"
          ${EndIf}
          ; Run the architecture dependant installer
          ${If} ${RunningX64}
            ExecWait "$TEMP\vcredist_x64_2008.exe" $0
          ${Else}
            ExecWait "$TEMP\vcredist_x86_2008.exe" $0
          ${EndIf}
          StrCmp $0 '0' +3 0
            MessageBox MB_YESNO "It appears that redistributable package might not have been installed properly. If you are sure everything is allright hit YES.$\nDo you want to continue the installation ?" IDYES +2
              Abort

        continueInstall:

        ;-----------------------------------------------------------------------------------
        ; The agent requires the following reg sub-key in order to install itself as
        ; a service
        ;-----------------------------------------------------------------------------------
        WriteRegStr HKLM SOFTWARE\ProActiveAgent "AgentLocation" "$INSTDIR"

        ;-----------------------------------------------------------------------------------
        ; The agent requires the following reg sub-key to know its default configuration
        ;-----------------------------------------------------------------------------------
        WriteRegStr HKLM SOFTWARE\ProActiveAgent "ConfigLocation" "$INSTDIR\PAAgent-config.xml"

        ;-----------------------------------------------------------------------------------
        ; Write the uninstall keys for Windows
        ;-----------------------------------------------------------------------------------
        WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent" "DisplayName" "ProActive Agent (remove only)"
        WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent" "UninstallString" '"$INSTDIR\Uninstall.exe"'

        SetOutPath $INSTDIR

        ; Write uninstaller utility
        WriteUninstaller uninstall.exe

        ; Write files
        File "LICENSE.txt"
        File "bin\Release\ConfigParser.exe"
        File "bin\Release\ProActiveAgent.exe"
        File "bin\Release\AgentFirstSetup.exe"
        File "ConfigParser\config.xsd"
        File "ProActiveAgent\lib\log4net.dll"
        File "ProActiveAgent\log4net.config"
        File "ProActiveAgent\lib\InJobProcessCreator.exe"
        ${If} ${RunningX64}
          File "ProActiveAgent\lib\x64\JobManagement.dll"
        ${Else}
          File "ProActiveAgent\lib\x86\JobManagement.dll"
        ${EndIf}
        File "utils\PAAgent-config.xml"
        File "utils\PAAgent-config-planning-day-only.xml"
        File "utils\PAAgent-config-planning-night-we.xml"
        File "utils\icon.ico"
        File "utils\ListNetworkInterfaces.class"
        File "documentation.pdf"

        ExecWait "$INSTDIR\AgentFirstSetup.exe -i $\"$INSTDIR$\""
SectionEnd
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section "ProActive Agent Control"
        SetOutPath $INSTDIR
        File "bin\Release\AgentForAgent.exe"
SectionEnd
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section "ProActive ScreenSaver"
        SetOutPath $SYSDIR
        File "bin\Release\ProActiveSSaver.scr"
SectionEnd
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section "Desktop shortcuts"
        SetShellVarContext all ; All users
        IfFileExists $INSTDIR\AgentForAgent.exe 0 +2
          CreateShortCut "$DESKTOP\ProActive Agent Control.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\icon.ico" 0
        SetShellVarContext current ; Current User
SectionEnd
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section "Start Menu Shortcuts"
        SetShellVarContext all ; All users
        CreateDirectory "$SMPROGRAMS\ProActiveAgent"
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Control.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\icon.ico" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\Uninstall ProActive Agent.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Documentation.lnk" "$INSTDIR\documentation.pdf" "" "$INSTDIR\documentation.pdf" 0
        SetShellVarContext current ; reset to current user

        ;; Ask user if he wants to run Agent GUI
        MessageBox MB_YESNO "Run ProActive Agent Control ?" /SD IDYES IDNO endActiveSync
          Exec "$INSTDIR\AgentForAgent.exe"
          Goto endActiveSync
         endActiveSync:
SectionEnd
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;uninstall section

UninstallText "This will uninstall ProActive Agent. Hit next to continue."

Section "Uninstall"

	MessageBox MB_OKCANCEL "This will delete $INSTDIR and all subdirectories and files?" IDOK DoUninstall
	
	Abort "Quiting the uninstall process"
	
	;-----------------------------------------------------------------------------------
        ; On x64 we have to explicitely set the registery view
        ;-----------------------------------------------------------------------------------
        ${If} ${RunningX64}
          SetRegView 64
        ${EndIf}
	
	DoUnInstall:
	SetOutPath $INSTDIR
	ExecWait "$INSTDIR\AgentFirstSetup.exe -u"
	
	; Delete from uninstall
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent"
	; Delete entry from auto start
        DeleteRegValue HKLM "Software\Microsoft\Windows\CurrentVersion\Run" "ProActiveAgent"
	DeleteRegKey HKLM SOFTWARE\ProActiveAgent
	
	SetShellVarContext all ; All users
	Delete "$DESKTOP\ProActive Agent Control.lnk"
	
	RMDir /r "$SMPROGRAMS\ProActiveAgent"
	SetShellVarContext current ; reset to current user
	
	Delete "$INSTDIR\AgentForAgent.exe"
	Delete "$INSTDIR\ConfigParser.exe"
        Delete "$INSTDIR\ProActiveAgent.exe"
       	RMDir /r $INSTDIR
	
	Delete $SYSDIR\ProActiveSSaver.scr

SectionEnd