#-----------------------------------------------------------------------------------
# Includes:
# - x64.nsh for few simple macros to handle installation on x64 architecture
# - DotNetVer.nsh for checking Microsoft .NET Framework versions (see http://ontheperiphery.veraida.com/project/dotnetver)
# - servicelib.nsh for Windows service installation
#-----------------------------------------------------------------------------------
!include x64.nsh
!include "DotNetVer.nsh"
!include "MUI.nsh"
!include "servicelib.nsh"

#-----------------------------------------------------------------------------------
# ProActive Agent service name constant
#-----------------------------------------------------------------------------------
!define SERVICE_NAME "ProActiveAgent"
!define SERVICE_DESC "The ProActive Agent enables desktop computers as an important source of computational power"
!define VERSION "2.2"
!define PAGE_FILE "serviceInstallPage.ini"
# Define the name of the service logon right
!define SERVICE_LOGON_RIGHT 'SeServiceLogonRight'

CRCCheck on

Name "ProActive Agent ${VERSION}"
OutFile ProActiveAgent-setup-${VERSION}.exe

LicenseText "This program is Licensed under the GNU General Public License (GPL)."
LicenseData "LICENSE.txt"

#-----------------------------------------------------------------------------------
# By default the installation directory is "Program Files" however
# on x64 architecture it will be translated as "Program Files (x86)"
#-----------------------------------------------------------------------------------
InstallDir $PROGRAMFILES\ProActiveAgent

ComponentText "This will install ProActive Agent on your computer. Select which optional components you want installed."

DirText "Choose a directory to install in to:"

Page License
Page Components
Page Directory
Page Instfiles
Page Custom MyCustomPage MyCustomLeave

Function MyCustomPage
  ReserveFile ${PAGE_FILE}
  !insertmacro MUI_INSTALLOPTIONS_EXTRACT ${PAGE_FILE}
  !insertmacro MUI_INSTALLOPTIONS_DISPLAY ${PAGE_FILE}
FunctionEnd

Function MyCustomLeave
  # R3 will store the Username, R4 the Password and R5 the Domain
  !insertmacro MUI_INSTALLOPTIONS_READ $R3 ${PAGE_FILE} "Field 4" "State"
  !insertmacro MUI_INSTALLOPTIONS_READ $R4 ${PAGE_FILE} "Field 5" "State"
  !insertmacro MUI_INSTALLOPTIONS_READ $R5 ${PAGE_FILE} "Field 8" "State"

  # R1 will store the value of "Install as LocalSystem"
  !insertmacro MUI_INSTALLOPTIONS_READ $R1 ${PAGE_FILE} "Field 6" "State"
  ${If} $R1 == "1"
        !insertmacro SERVICE "create" ${SERVICE_NAME} "path=$INSTDIR\ProActiveAgent.exe;autostart=1;interact=0;display=${SERVICE_NAME};description=${SERVICE_DESC};" ""
  ${EndIf}
  # R1 will store the value of "Specify an account"
  !insertmacro MUI_INSTALLOPTIONS_READ $R1 ${PAGE_FILE} "Field 7" "State"
  ${If} $R1 == "1"
        # Check for empty username
        ${If} $R3 == ""
          MessageBox MB_OK "Please enter a valid account name"
            Abort # Go back to page.
        # Check for empty password
        ${Else}
          ${If} $R4 == ""
            MessageBox MB_OK "Please enter a valid password"
              Abort # Go back to page.
          ${Else}
            # Check for service logon right
            UserMgr::HasPrivilege $R3 ${SERVICE_LOGON_RIGHT}
            Pop $0
            ${If} $0 == "True"
                  Goto installService
            ${EndIf}
            # If FALSE that means the account does not have the service logon right
            ${If} $0 == "FALSE" 
               DetailPrint "User $R3 does not have the service logon right !"
               MessageBox MB_OK "The user $R3 does not have the log on service right assignment. In the 'Administrative Tools' of the 'Control Panel' open the 'Local Security Policy'. In 'Security Settings', select 'Local Policies' then select 'User Rights Assignments'. Finally, in the list of policies open the properties of 'Log on as a service' policy and add the user $R3."
                 Abort # Go back to page.
            ${EndIf}
            # Treat specific error ... the account does not exist
            ${If} $0 == "ERROR GetAccountSid" 
               DetailPrint "The account $R3 does not exist ... asking user if he wants to create a new account"
               # Ask the user if he wants to create a new account
               MessageBox MB_YESNO "The account $R3 does not exist, would you like to create it ?" IDYES createAccount
                Abort
               createAccount:
               UserMgr::CreateAccount $R3 $R4 "The ProActive Agent service may be started under this account."
  	       Pop $0
               ${If} $0 == "ERROR 2224" # Means account already exists .. it's strange but yes it is possible !
                  DetailPrint "The account $R3 already exist"
                  MessageBox MB_OK "The account $R3 already exist"
                    Abort
        	${EndIf}
        	# Add service log on privilege
        	UserMgr::AddPrivilege $R3 ${SERVICE_LOGON_RIGHT}
        	Pop $0
        	${If} $0 == "FALSE" # Means could not add privilege
                  DetailPrint "Unable to add privileges"
                  MessageBox MB_OK "Unable to add privileges"
                    Abort
        	${EndIf}
            # Unknown error just print and still try to install the user
            ${Else}
               DetailPrint "Unable to check for service logon right due to $0, still trying to install the service"
               MessageBox MB_OK "Unable to check for service logon right due to $0"
            ${EndIf}
            installService:
            !insertmacro SERVICE "create" ${SERVICE_NAME} "path=$INSTDIR\ProActiveAgent.exe;autostart=1;interact=0;display=${SERVICE_NAME};description=${SERVICE_DESC};user=$R5\$R3;password=$R4;" ""
            Pop $0
            # Means the service is not installed !
            ${If} $0 != "true"
              DetailPrint "Unable to install as service."
              MessageBox MB_OK "Unable to install as service. To install manually use sc.exe command"
           ${EndIf}
          ${EndIf}
        ${EndIf}
  ${EndIf}
FunctionEnd

############################################################################################
# On init peforms the following checks:
# - admin rights
# - Microsoft .NET Framework 3.5
# - VC++ redist 2008
# - Previous version of the unistaller
############################################################################################
Function .onInit

        #-----------------------------------------------------------------------------------
        # Check user admin rights
        #-----------------------------------------------------------------------------------
        System::Call "kernel32::GetModuleHandle(t 'shell32.dll') i .s"
        System::Call "kernel32::GetProcAddress(i s, i 680) i .r0"
        System::Call "::$0() i .r0"
        DetailPrint "Check: Current user is admin? $0"
        StrCmp $0 '0' 0 +3
          MessageBox MB_OK "Adminstrator rights are required to install the ProActive Agent."
          Abort

        #-----------------------------------------------------------------------------------
        # On x64 we have to explicitely set the registery view
        #-----------------------------------------------------------------------------------
        ${If} ${RunningX64}
          SetRegView 64
        ${EndIf}

        #-----------------------------------------------------------------------------------
        # Check if .NET framework 3.5 is installed
        #-----------------------------------------------------------------------------------
        ${IfNot} ${HasDotNet3.5}
            MessageBox MB_OK "Microsoft .NET Framework 3.5 is required."
            Abort
        ${EndIf}

        #-----------------------------------------------------------------------------------
        # Check if VC++ redist 2008 is installed
        #-----------------------------------------------------------------------------------
        ReadRegDWORD $0 HKLM Software\Microsoft\DevDiv\VC\Servicing\9.0\RED\1033 Install
        # If the redistributable package is not installed run the installer
        StrCmp $0 '1' continueInstall 0
          MessageBox MB_OK 'You must install the Visual C++ 2008 Redistributable Package to use ProActive Agent.$\nPress OK to begin installation.'
          # Prepare to copy the redistributable package
          SetOutPath $TEMP
          # Copy the architecture dependant installer
          ${If} ${RunningX64}
          File "utils\x64\vcredist_x64_2008.exe"
          ${Else}
          File "utils\x86\vcredist_x86_2008.exe"
          ${EndIf}
          # Run the architecture dependant installer
          ${If} ${RunningX64}
            ExecWait "$TEMP\vcredist_x64_2008.exe" $0
          ${Else}
            ExecWait "$TEMP\vcredist_x86_2008.exe" $0
          ${EndIf}
          StrCmp $0 '0' +3 0
            MessageBox MB_YESNO "It appears that redistributable package might not have been installed properly. If you are sure everything is allright hit YES.$\nDo you want to continue the installation ?" IDYES +2
            Abort
        continueInstall:
        
        #-----------------------------------------------------------------------------------
        # Check if User Account Protection is Activated (Windows Vista)
        #-----------------------------------------------------------------------------------
        # ReadRegDWORD $1 HKLM Software\Microsoft\Windows\CurrentVersion\Policies\System EnableLUA
        # StrCmp $1 '1' 0 +3
        #  MessageBox MB_OK "It appears that the User Account Control (UAC) feature is enabled. The installation cannot continue. Please disable the UAC feature and restart the installation. To disable the UAC feature: Go to the User Accounts part in the Control Panel and click on the 'Turn User Account Control on or off' Next, uncheck the 'Use User Account' check box to disable and reboot."
        #    Abort

        #-----------------------------------------------------------------------------------
        # Check if a previous version of the unistaller is available
       	#-----------------------------------------------------------------------------------
        IfFileExists $INSTDIR\uninstall.exe 0 endLabel
        
        #-----------------------------------------------------------------------------------
        # Ask the user if he wants to uninstall previous version
       	#-----------------------------------------------------------------------------------
        MessageBox MB_YESNO "The previous version of the ProActive Windows Agent must be uninstalled. Run the uninstaller ?" /SD IDYES IDNO abortLabel
        Exec $INSTDIR\uninstall.exe
        Goto endLabel
        
        abortLabel:
        Abort
        
        endLabel:

FunctionEnd

######################################
Section "ProActive Agent"
        
        #-----------------------------------------------------------------------------------
        # The agent requires the following reg sub-key in order to install itself as
        # a service
        #-----------------------------------------------------------------------------------
        WriteRegStr HKLM "Software\ProActiveAgent" "AgentLocation" "$INSTDIR"

        #-----------------------------------------------------------------------------------
        # Write the uninstall keys for Windows
        #-----------------------------------------------------------------------------------
        WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent" "DisplayName" "ProActive Agent (remove only)"
        WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent" "UninstallString" '"$INSTDIR\Uninstall.exe"'

       	#-----------------------------------------------------------------------------------
        # Set current dir to installation directory
        #-----------------------------------------------------------------------------------
        SetOutPath $INSTDIR

        #-----------------------------------------------------------------------------------
        # Write uninstaller utility
        #-----------------------------------------------------------------------------------
        WriteUninstaller uninstall.exe

        #-----------------------------------------------------------------------------------
        # Write files
        #-----------------------------------------------------------------------------------
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

        #-----------------------------------------------------------------------------------
        # The agent requires the following reg sub-key to know its default configuration
        #-----------------------------------------------------------------------------------
        WriteRegStr HKLM "Software\ProActiveAgent" "ConfigLocation" "$INSTDIR\config\PAAgent-config.xml"
        
        #-----------------------------------------------------------------------------------
        # Run the internal service installer
        #-----------------------------------------------------------------------------------
        ;ExecWait "$INSTDIR\AgentFirstSetup.exe -i $\"$INSTDIR$\""
        
        #-----------------------------------------------------------------------------------
        # Copy the GUI
        #-----------------------------------------------------------------------------------
        SetOutPath $INSTDIR
        File "bin\Release\AgentForAgent.exe"
SectionEnd
######################################

######################################
Section "ProActive ScreenSaver"
        SetOutPath $SYSDIR
        File "bin\Release\ProActiveSSaver.scr"
SectionEnd
######################################

######################################
# Section "Desktop shortcuts"
#         SetShellVarContext all # All users
#         IfFileExists $INSTDIR\AgentForAgent.exe 0 +2
#           CreateShortCut "$DESKTOP\ProActive Agent Control.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\icon.ico" 0
#         SetShellVarContext current # Current User
# SectionEnd
######################################

######################################
Section "Start Menu Shortcuts"
        SetShellVarContext all # All users
        CreateDirectory "$SMPROGRAMS\ProActiveAgent"
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Control.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\icon.ico" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\Uninstall ProActive Agent.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Documentation.lnk" "$INSTDIR\ProActive Agent Documentation.pdf" "" "$INSTDIR\ProActive Agent Documentation.pdf" 0
        SetShellVarContext current # reset to current user

        # Ask user if he wants to run Agent GUI
        ;MessageBox MB_YESNO "Run ProActive Agent Control and exit installer?" /SD IDYES IDNO endActiveSync
        ;  Exec "$INSTDIR\AgentForAgent.exe"
        ;  Quit
        ; endActiveSync:
SectionEnd
######################################

#uninstall section

UninstallText "This will uninstall ProActive Agent. Hit next to continue."

Section "Uninstall"
        #-----------------------------------------------------------------------------------
        # For all users
       	#-----------------------------------------------------------------------------------
	SetShellVarContext all
      	  
	MessageBox MB_OKCANCEL "This will delete $INSTDIR and all subdirectories and files?" IDOK DoUninstall
	Abort "Quiting the uninstall process"
	DoUnInstall:
	
        #-----------------------------------------------------------------------------------
        # Ask the user if he wants to keep the configuration files
       	#-----------------------------------------------------------------------------------
        MessageBox MB_YESNO "Delete configuration files from $INSTDIR\config ?" /SD IDYES IDNO keepConfigLabel
     	  SetOutPath $INSTDIR\config
          Delete "PAAgent-config.xml"
          Delete "PAAgent-config-planning-day-only.xml"
          Delete "PAAgent-config-planning-night-we.xml"
      	  SetOutPath $INSTDIR
      	  RMDir /r "$INSTDIR\config"
        keepConfigLabel:
	
	#-----------------------------------------------------------------------------------
        # On x64 we have to explicitely set the registery view
        #-----------------------------------------------------------------------------------
        ${If} ${RunningX64}
          SetRegView 64
        ${EndIf}
        
	SetOutPath $INSTDIR
	#-----------------------------------------------------------------------------------
        # Check if the service is installed and delete it
        #-----------------------------------------------------------------------------------
        !insertmacro SERVICE "stop" ${SERVICE_NAME} "" "un."
        !insertmacro SERVICE "delete" ${SERVICE_NAME} "" "un."
	#ExecWait "$INSTDIR\AgentFirstSetup.exe -u"
	
	# Delete regkey from uninstall
	DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent"
	# Delete entry from auto start
        DeleteRegValue HKLM "Software\Microsoft\Windows\CurrentVersion\Run" "ProActiveAgent"
	DeleteRegKey HKLM "Software\ProActiveAgent"
	
	#-----------------------------------------------------------------------------------
	# Remove the screen saver
	#-----------------------------------------------------------------------------------
       	Delete $SYSDIR\ProActiveSSaver.scr
        
       	#-----------------------------------------------------------------------------------
	# Remove all known files except config directory from $INSTDIR
	#-----------------------------------------------------------------------------------
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
	SetShellVarContext current # reset to current user
	
SectionEnd