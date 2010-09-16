;-----------------------------------------------------------------------------------
; Includes:
; - x64.nsh few simple macros to handle installation on x64 architecture
; - DotNetVer.nsh for checking Microsoft .NET Framework versions (see http://ontheperiphery.veraida.com/project/dotnetver)
;-----------------------------------------------------------------------------------
!include x64.nsh
!include "DotNetVer.nsh"

CRCCheck on

Name "ProActive Agent 2.2"
OutFile ProActiveAgent-setup-v2.2.exe

LicenseText "This program is Licensed under the GNU General Public License (GPL)."
LicenseData "LICENSE.txt"

;-----------------------------------------------------------------------------------
; By default the installation directory is "Program Files" however
; on x64 architecture it will be translated as "Program Files (x86)"
;-----------------------------------------------------------------------------------
InstallDir $PROGRAMFILES\ProActiveAgent

;-----------------------------------------------------------------------------------
; A variable to contain the ProActive Agent service user
;-----------------------------------------------------------------------------------
var ServiceUser

ComponentText "This will install ProActive Agent on your computer. Select which optional components you want installed."

DirText "Choose a directory to install in to:"

Page license
Page components
Page directory
Page instfiles

############################################################################################
# On init peforms the following checks:
# - admin rights
# - Microsoft .NET Framework 3.5
# - VC++ redist 2008
# - Previous version of the unistaller
############################################################################################
Function .onInit

        ;-----------------------------------------------------------------------------------
        ; Check user admin rights
        ;-----------------------------------------------------------------------------------
        System::Call "kernel32::GetModuleHandle(t 'shell32.dll') i .s"
        System::Call "kernel32::GetProcAddress(i s, i 680) i .r0"
        System::Call "::$0() i .r0"
        DetailPrint "Check: Current user is admin? $0"
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
        ; Check if .NET framework 3.5 is installed
        ;-----------------------------------------------------------------------------------
        ${IfNot} ${HasDotNet3.5}
            MessageBox MB_OK "Microsoft .NET Framework 3.5 is required."
            Abort
        ${EndIf}
        
        ;-----------------------------------------------------------------------------------
        ; Check if VC++ redist 2008 is installed
        ;-----------------------------------------------------------------------------------
        ReadRegDWORD $0 HKLM Software\Microsoft\DevDiv\VC\Servicing\9.0\RED\1033 Install
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
        ; Check if User Account Protection is Activated (Windows Vista)
        ;-----------------------------------------------------------------------------------
        ; ReadRegDWORD $1 HKLM Software\Microsoft\Windows\CurrentVersion\Policies\System EnableLUA
        ; StrCmp $1 '1' 0 +3
        ;  MessageBox MB_OK "It appears that the User Account Control (UAC) feature is enabled. The installation cannot continue. Please disable the UAC feature and restart the installation. To disable the UAC feature: Go to the User Accounts part in the Control Panel and click on the 'Turn User Account Control on or off' Next, uncheck the 'Use User Account' check box to disable and reboot."
        ;    Abort

        ;-----------------------------------------------------------------------------------
        ; Check if a previous version of the unistaller is available
       	;-----------------------------------------------------------------------------------
        IfFileExists $INSTDIR\uninstall.exe 0 endLabel
        
        ;-----------------------------------------------------------------------------------
        ; Ask the user if he wants to uninstall previous version
       	;-----------------------------------------------------------------------------------
        MessageBox MB_YESNO "The previous version of the ProActive Windows Agent must be uninstalled. Run the uninstaller ?" /SD IDYES IDNO abortLabel
        Exec $INSTDIR\uninstall.exe
        Goto endLabel
        
        abortLabel:
        Abort
        
        endLabel:

FunctionEnd

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section "ProActive Agent"
        
        ;-----------------------------------------------------------------------------------
        ; The agent requires the following reg sub-key in order to install itself as
        ; a service
        ;-----------------------------------------------------------------------------------
        WriteRegStr HKLM SOFTWARE\ProActiveAgent "AgentLocation" "$INSTDIR"

        ;-----------------------------------------------------------------------------------
        ; Write the uninstall keys for Windows
        ;-----------------------------------------------------------------------------------
        WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent" "DisplayName" "ProActive Agent (remove only)"
        WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent" "UninstallString" '"$INSTDIR\Uninstall.exe"'

       	;-----------------------------------------------------------------------------------
        ; Set current dir to installation directory
        ;-----------------------------------------------------------------------------------
        SetOutPath $INSTDIR

        ;-----------------------------------------------------------------------------------
        ; Write uninstaller utility
        ;-----------------------------------------------------------------------------------
        WriteUninstaller uninstall.exe

        ;-----------------------------------------------------------------------------------
        ; Write files
        ;-----------------------------------------------------------------------------------
        File "LICENSE.txt"
        File "bin\Release\ConfigParser.dll"
        File "bin\Release\ConfigParserOLD.dll"
        File "bin\Release\ProActiveAgent.exe"
        File "bin\Release\AgentFirstSetup.exe"
        File "utils\icon.ico"
        File "utils\ListNetworkInterfaces.class"
        File "ProActive Agent Documentation.pdf"
        File "ProActiveAgent\lib\log4net.dll"
        File "ProActiveAgent\log4net.config"
        File "ProActiveAgent\lib\InJobProcessCreator.exe"
        ${If} ${RunningX64}
          File "ProActiveAgent\lib\x64\JobManagement.dll"
        ${Else}
          File "ProActiveAgent\lib\x86\JobManagement.dll"
        ${EndIf}

        IfFileExists $INSTDIR\config\PAAgent-config.xml 0 defaultFileNotExistLabel

        MessageBox MB_YESNO "Use existing configuration file $INSTDIR\config\PAAgent-config.xml ?" /SD IDYES IDNO defaultFileNotExistLabel
        Goto continueInstallLabel

        defaultFileNotExistLabel:
        SetOutPath $INSTDIR\config
        File "utils\PAAgent-config.xml"

        continueInstallLabel:
        SetOutPath $INSTDIR\config
        File "utils\PAAgent-config-planning-day-only.xml"
        File "utils\PAAgent-config-planning-night-we.xml"
        SetOutPath $INSTDIR\xml
        File "utils\xml\agent-windows.xsd"
        File "utils\xml\agent-common.xsd"
        File "utils\xml\agent-old.xsd"

        ;-----------------------------------------------------------------------------------
        ; The agent requires the following reg sub-key to know its default configuration
        ;-----------------------------------------------------------------------------------
        WriteRegStr HKLM SOFTWARE\ProActiveAgent "ConfigLocation" "$INSTDIR\config\PAAgent-config.xml"
        
        ;-----------------------------------------------------------------------------------
        ; Run the internal service installer
        ;-----------------------------------------------------------------------------------
        ExecWait "$INSTDIR\AgentFirstSetup.exe -i $\"$INSTDIR$\""

        ;-----------------------------------------------------------------------------------
        ; Read the service user from the registry. If there is no service user specified
        ; then we assume that the service user is LocalSystem so no need to check the rights.
        ;-----------------------------------------------------------------------------------
        ReadRegDWORD $ServiceUser HKLM SOFTWARE\ProActiveAgent ServiceUser
        StrCmp $ServiceUser '' noServiceUserLabel
          Goto doneLabel
        noServiceUserLabel:
          DetailPrint "No service user was specified, assuming service user is LocalSystem"
          Goto endLabel
        doneLabel:
        
        ;-----------------------------------------------------------------------------------
        ; The following code will enumerate the rights that a given user has.
        ; See http://nsis.sourceforge.net/Enumerate_User_Privileges for more details
        ;-----------------------------------------------------------------------------------
        !define POLICY_LOOKUP_NAMES 0x00000800
        !define strLSA_OBJECT_ATTRIBUTES '(i,i,w,i,i,i)i'
        !define strLSA_UNICODE_STRING '(&i2,&i2,w)i'

        System::Call '*${strLSA_OBJECT_ATTRIBUTES}(24,n,n,0,n,n).r0'
        System::Call 'advapi32::LsaOpenPolicy(w n, i r0, i ${POLICY_LOOKUP_NAMES}, *i .R0) i.R8'
        StrCpy $2 $ServiceUser
        StrCpy $3 ${NSIS_MAX_STRLEN}
        System::Call '*(&w${NSIS_MAX_STRLEN})i.R1'
        System::Call 'Advapi32::LookupAccountNameW(w n, w r2, i R1, *i r3, w .R8, *i r3, *i .r4) i .R8'

        ;-----------------------------------------------------------------------------------
        ; Enumerate the rights
        ; R2 is the pointer to an array of LSA_UNICODE_STRING structures
        ; R3 is a variable that receives the number of privileges in the R2 array
        ;-----------------------------------------------------------------------------------
        System::Call 'advapi32::LsaEnumerateAccountRights(i R0, i R1, *i .R2, *i .R3)i.R8'
        System::Call 'advapi32::LsaNtStatusToWinError(i R8) i.R9'

        ; Define the name of the service logon right
        !define strSLR 'SeServiceLogonRight'

        # Get the rights out to $4
        StrCpy $9 0
        loop:
         StrCmp $9 $R3 stop
         System::Call '*$R2${strLSA_UNICODE_STRING}(.r2,.r3,.r4)'
         StrCmp $4 ${strSLR} stop
         IntOp $R2 $R2 + 8
         IntOp $9 $9 + 1
        stop:
        
        ;-----------------------------------------------------------------------------------
        ; If the right was not found print a warning message that explains how to add
        ; the service logon right.
        ;-----------------------------------------------------------------------------------
        StrCmp $4 ${strSLR} slrFoundLabel
          DetailPrint 'User $ServiceUser does not have the service logon right!'
          MessageBox MB_OK "The user $ServiceUser does not have the log on service right assignment. In the 'Administrative Tools' of the 'Control Panel' open the 'Local Security Policy'. In 'Security Settings', select 'Local Policies' then select 'User Rights Assignments'. Finally, in the list of policies open the properties of 'Log on as a service' policy and add the user $ServiceUser."
        slrFoundLabel:

        System::Free $0
        System::Free $R1
        System::Call 'advapi32::LsaFreeMemory(i R2) i .R8'
        System::Call 'advapi32::LsaClose(i R0) i .R8'
        
        endLabel:
        
        ;-----------------------------------------------------------------------------------
        ; Copy the GUI
        ;-----------------------------------------------------------------------------------
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
; Section "Desktop shortcuts"
;         SetShellVarContext all ; All users
;         IfFileExists $INSTDIR\AgentForAgent.exe 0 +2
;           CreateShortCut "$DESKTOP\ProActive Agent Control.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\icon.ico" 0
;         SetShellVarContext current ; Current User
; SectionEnd
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
Section "Start Menu Shortcuts"
        SetShellVarContext all ; All users
        CreateDirectory "$SMPROGRAMS\ProActiveAgent"
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Control.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\icon.ico" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\Uninstall ProActive Agent.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Documentation.lnk" "$INSTDIR\ProActive Agent Documentation.pdf" "" "$INSTDIR\ProActive Agent Documentation.pdf" 0
        SetShellVarContext current ; reset to current user

        ;; Ask user if he wants to run Agent GUI
        MessageBox MB_YESNO "Run ProActive Agent Control and exit installer?" /SD IDYES IDNO endActiveSync
          Exec "$INSTDIR\AgentForAgent.exe"
          Quit
         endActiveSync:
SectionEnd
;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

;uninstall section

UninstallText "This will uninstall ProActive Agent. Hit next to continue."

Section "Uninstall"
        ;-----------------------------------------------------------------------------------
        ; For all users
       	;-----------------------------------------------------------------------------------
	SetShellVarContext all
      	  
	MessageBox MB_OKCANCEL "This will delete $INSTDIR and all subdirectories and files?" IDOK DoUninstall
	Abort "Quiting the uninstall process"
	DoUnInstall:
	
        ;-----------------------------------------------------------------------------------
        ; Ask the user if he wants to keep the configuration files
       	;-----------------------------------------------------------------------------------
        MessageBox MB_YESNO "Delete configuration files from $INSTDIR\config ?" /SD IDYES IDNO keepConfigLabel
     	  SetOutPath $INSTDIR\config
          Delete "PAAgent-config.xml"
          Delete "PAAgent-config-planning-day-only.xml"
          Delete "PAAgent-config-planning-night-we.xml"
      	  SetOutPath $INSTDIR
      	  RMDir /r "$INSTDIR\config"
        keepConfigLabel:
	
	;-----------------------------------------------------------------------------------
        ; On x64 we have to explicitely set the registery view
        ;-----------------------------------------------------------------------------------
        ${If} ${RunningX64}
          SetRegView 64
        ${EndIf}
        
	SetOutPath $INSTDIR
	;-----------------------------------------------------------------------------------
        ; Call AgentFirstSetup to unistall the service
        ;-----------------------------------------------------------------------------------
	ExecWait "$INSTDIR\AgentFirstSetup.exe -u"
	
	; Delete regkey from uninstall
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent"
	; Delete entry from auto start
        DeleteRegValue HKLM "Software\Microsoft\Windows\CurrentVersion\Run" "ProActiveAgent"
	DeleteRegKey HKLM SOFTWARE\ProActiveAgent
	
	;-----------------------------------------------------------------------------------
	; Remove the screen saver
	;-----------------------------------------------------------------------------------
       	Delete $SYSDIR\ProActiveSSaver.scr
        
       	;-----------------------------------------------------------------------------------
	; Remove all known files except config directory from $INSTDIR
	;-----------------------------------------------------------------------------------
        Delete "$INSTDIR\xml\agent-windows.xsd"
        Delete "$INSTDIR\xml\agent-common.xsd"
        Delete "$INSTDIR\xml\agent-old.xsd"
	RMDir /r "$INSTDIR\xml"
        Delete "ConfigParser.dll"
        Delete "ConfigParserOLD.dll"
        Delete "ProActiveAgent.exe"
        Delete "AgentFirstSetup.exe"
        Delete "log4net.dll"
        Delete "log4net.config"
        Delete "InJobProcessCreator.exe"
        Delete "JobManagement.dll"
        Delete "icon.ico"
        Delete "ListNetworkInterfaces.class"
        Delete "ProActive Agent Documentation.pdf"
        Delete "LICENSE.txt"
        Delete "configuration.ini"
        Delete "uninstall.exe"
        Delete "ProActiveAgent-log.txt"
        Delete "AgentForAgent.exe"

	RMDir /r "$SMPROGRAMS\ProActiveAgent"
	SetShellVarContext current ; reset to current user
	
SectionEnd