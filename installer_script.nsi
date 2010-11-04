############################################################################################################################
# Includes:
# - x64.nsh for few simple macros to handle installation on x64 architecture
# - DotNetVer.nsh for checking Microsoft .NET Framework versions (see http://ontheperiphery.veraida.com/project/dotnetver)
# - servicelib.nsh for Windows service installation
# - UserManagement.nsh contains GetServerName and AddUserToGroup1 from http://nsis.sourceforge.net/User_Management_using_API_calls#Add_User_to_a_group
############################################################################################################################
!include x64.nsh
!include "DotNetVer.nsh"
!include "MUI.nsh"
!include "servicelib.nsh"
!include "NTProfiles.nsh"
!include "UserManagement.nsh"
!include WinVer.nsh

#################################################################
# Some constants definitions like service name, version, etc ...
#################################################################
!define SERVICE_NAME "ProActiveAgent"
!define SERVICE_DESC "The ProActive Agent enables desktop computers as an important source of computational power"
!define VERSION "2.3"
!define PAGE_FILE "serviceInstallPage.ini"
!define SUBINACL_DIR "$PROGRAMFILES\Windows Resource Kits\Tools"
!define SUBINACL_PATH "${SUBINACL_DIR}\subinacl.exe"
!define SUBINACL_URL "http://download.microsoft.com/download/1/7/d/17d82b72-bc6a-4dc8-bfaa-98b37b22b367/subinacl.msi"
!define SUBINACL_MANUAL_INSTALL "Please download SubInAcl.msi and install it manually then run the command: subinacl.exe /service ${SERVICE_NAME} /grant=S-1-1-0=TO"
!define SERVICE_LOGON_RIGHT 'SeServiceLogonRight'
!define CONFIG_NAME "PAAgent-config.xml"
!define DEFAULT_CONFIG_PATH "$INSTDIR\config\${CONFIG_NAME}"
!define PERFORMANCE_MONITOR_SID "S-1-5-32-558"
!define TXT_LOGSDIR "Field 13"
!define TXT_CONF "Field 14"
!define SEL_INSTLOC "Field 6"
!define SEL_SPECREAC "Field 7"
!define CHK_LOGSHOME "Field 16"

Var Hostname
Var tmp

CRCCheck on

Name "ProActive Agent ${VERSION}"
OutFile ProActiveAgent-setup-${VERSION}.exe

LicenseText "This program is Licensed under the GNU General Public License (GPL)."
LicenseData "LICENSE.txt"

#####################################################################
# By default the installation directory is "Program Files" however
# on x64 architecture it will be translated as "Program Files (x86)"
#####################################################################
InstallDir $PROGRAMFILES\ProActiveAgent

ComponentText "This will install ProActive Agent on your computer. Select which optional components you want installed."

DirText "Choose a directory to install in to:"

Page License
Page Components
Page Directory
Page Instfiles
# The page that specifies locations and service account
Page Custom MyCustomPage MyCustomLeave


!include "WordFunc.nsh"
!insertmacro WordReplace
!insertmacro WordFind

; Activate a group of controls, depending on the state of one control
;
; Usage:
;
; eg. !insertmacro GROUPCONTROLS "${DIALOG1}" "${CHK_PROXYSETTINGS}" "${LBL_IPADDRESS}|${TXT_IPADDRESS}|${LBL_PORT1}|${TXT_PORT1}|${CHK_ENCRYPTION}"
; FILE:          INI-file in $pluginsdir
; SOURCECONTROL: RadioButton, Checkbox
; CONTROLGROUP:  pipe delimited list of controls; ${BUTTON1}|${CHECKBOX}|${TEXTFIELD}
;
; Requires:
;
; !include "WordFunc.nsh"
; !insertmacro WordReplace
; !insertmacro WordFind
;
!macro GROUPCONTROLS FILE SOURCECONTROL CONTROLGROUP INV
  Push $R0 ;holds element
  Push $R1 ;counter
  Push $R2 ;state of the control
  Push $R3 ;flags of the control / hwnd of the control

  !insertmacro MUI_INSTALLOPTIONS_READ $R2 "${FILE}" "${SOURCECONTROL}" "State"
  
  ${If} ${INV} == "1"
    ${If} "$R2" == "1"
      StrCpy $R2 0
    ${Else}
      StrCpy $R2 1
    ${EndIf}
  ${EndIf}

  StrCpy $R1 1
  ${Do}
    ClearErrors
    ${WordFind} "${CONTROLGROUP}" "|" "E+$R1" $R0

    ${If} ${Errors}
    ${OrIf} $R0 == ""
      ${ExitDo}
    ${EndIf}

    ; Put state change in flags of element as well
    !insertmacro MUI_INSTALLOPTIONS_READ $R3 "${FILE}" "$R0" "Flags"
    ${If} "$R2" == "1"
       ${WordReplace} $R3 "DISABLED" "" "+" $R3
       ${WordReplace} $R3 "||" "|" "+" $R3
      !insertmacro MUI_INSTALLOPTIONS_WRITE "${FILE}" "$R0" "Flags" $R3
    ${Else}
      !insertmacro MUI_INSTALLOPTIONS_WRITE "${FILE}" "$R0" "Flags" "$R3|DISABLED"
    ${EndIf}

    !insertmacro MUI_INSTALLOPTIONS_READ $R3 "${FILE}" "$R0" "HWND"
    EnableWindow $R3 $R2

    IntOp $R1 $R1 + 1
  ${Loop}

  Pop $R3
  Pop $R2
  Pop $R1
  Pop $R0

!macroend

########################################
# On init peforms the following checks:
# - admin rights
# - Microsoft .NET Framework 3.5
# - VC++ redist 2008
# - Previous version of the unistaller
########################################
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
        ## ReadRegDWORD $1 HKLM Software\Microsoft\Windows\CurrentVersion\Policies\System EnableLUA
        ## StrCmp $1 '1' 0 +3
        ##  MessageBox MB_OK "It appears that the User Account Control (UAC) feature is enabled. The installation cannot continue. Please disable the UAC feature and restart the installation. To disable the UAC feature: Go to the User Accounts part in the Control Panel and click on the 'Turn User Account Control on or off' Next, uncheck the 'Use User Account' check box to disable and reboot."
        ##    Abort

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
        # Here we go to the custom page
FunctionEnd

Function MyCustomPage
  ReserveFile ${PAGE_FILE}
  !insertmacro MUI_INSTALLOPTIONS_EXTRACT ${PAGE_FILE}
   # Set default location for configuration file
  !insertmacro MUI_INSTALLOPTIONS_WRITE ${PAGE_FILE} "Field 14" State "$INSTDIR\config"
   # Set default location for logs directory
  !insertmacro MUI_INSTALLOPTIONS_WRITE ${PAGE_FILE} "Field 13" State "$INSTDIR\logs"
  # Disable "Use service account home"
  !insertmacro GROUPCONTROLS "${PAGE_FILE}" "${SEL_INSTLOC}" "${CHK_LOGSHOME}|" "1"
  # Display the custom page
  !insertmacro MUI_INSTALLOPTIONS_DISPLAY ${PAGE_FILE}
FunctionEnd

Function MyCustomLeave

  # Handle notify event of checkbox "Use service account home"
  
  !insertmacro MUI_INSTALLOPTIONS_READ $tmp "${PAGE_FILE}" "Settings" "State"
  ${Switch} "Field $tmp"
    ${Case} "${CHK_LOGSHOME}"
      !insertmacro GROUPCONTROLS "${PAGE_FILE}" "${CHK_LOGSHOME}" "${TXT_LOGSDIR}|${TXT_CONF}" "1"
      Abort
    ${Case} "${SEL_INSTLOC}"
      !insertmacro GROUPCONTROLS "${PAGE_FILE}" "${SEL_INSTLOC}" "${CHK_LOGSHOME}|" "1"
      Abort
    ${Case} "${SEL_SPECREAC}"
      !insertmacro GROUPCONTROLS "${PAGE_FILE}" "${SEL_SPECREAC}" "${CHK_LOGSHOME}|" "0"
      Abort
  ${EndSwitch}


  #-----------------------------------------------------------------------------------
  # !! CHECK LOCATIONS !!
  #-----------------------------------------------------------------------------------
  
  # Check "Use service account home" for logs location stored in R7
  !insertmacro MUI_INSTALLOPTIONS_READ $R7 ${PAGE_FILE} "${CHK_LOGSHOME}" State
  ${If} $R7 == "1"
    #MessageBox MB_OK "--------------> "
    Goto skipLocationsLABEL
  ${EndIf}

  # Check config file location stored in R1
  !insertmacro MUI_INSTALLOPTIONS_READ $R1 ${PAGE_FILE} "${TXT_CONF}" State
  ${If} $R1 == ""
    MessageBox MB_OK "Please enter a valid location for the Configuration File"
     Abort
  ${Else}
    # If the location is NOT THE DEFAULT ONE
    ${If} $R1 != "$INSTDIR\config"
      # Check if the specified directory exists
      IfFileExists $R1 dirExistLABEL dirNotExistLABEL
      dirNotExistLABEL:
      MessageBox MB_OK "The specified directory $R1 for configuration file doesn't exist"
        Abort
      dirExistLABEL:
      # Check if there is already a config file
      IfFileExists "$R1\${CONFIG_NAME}" askUseLABEL copyDefaultLABEL
      askUseLABEL:
      # Ask the user if he wants to use the existing file (if not the default one will be copied to this dir)
      MessageBox MB_YESNO "Use existing configuration file $R1\${CONFIG_NAME} ?" IDYES setLocationLABEL
      copyDefaultLABEL:
      SetOutPath $R1
      File "utils\PAAgent-config.xml"
      setLocationLABEL:
      StrCpy $R1 "$R1\${CONFIG_NAME}" # R1 will contain the full path
    ${Else}
      StrCpy $R1 "$INSTDIR\config\${CONFIG_NAME}"
    ${EndIf}
  ${EndIf}

  # Check logs location stored in R2
  !insertmacro MUI_INSTALLOPTIONS_READ $R2 ${PAGE_FILE} "${TXT_LOGSDIR}" State
  ${If} $R2 == ""
    MessageBox MB_OK "Please enter a valid location for the logs"
     Abort
  ${EndIf}

  skipLocationsLABEL:

  #-----------------------------------------------------------------------------------
  # !! SELECTED: Install as LocalSystem !!
  #-----------------------------------------------------------------------------------

  # R1 will store the value of "Install as LocalSystem"
  !insertmacro MUI_INSTALLOPTIONS_READ $R0 ${PAGE_FILE} "Field 6" State
  ${If} $R0 == "1"
    !insertmacro SERVICE "create" ${SERVICE_NAME} "path=$INSTDIR\ProActiveAgent.exe ;autostart=1;interact=0;display=${SERVICE_NAME};description=${SERVICE_DESC};" ""
    goto writeToRegistryLABEL
  ${EndIf}

  #-----------------------------------------------------------------------------------
  # !! CHECK ACCOUNT FIELDS !!
  #-----------------------------------------------------------------------------------

  # Check for empty username stored in R3
  !insertmacro MUI_INSTALLOPTIONS_READ $R3 ${PAGE_FILE} "Field 4" "State"
  ${If} $R3 == ""
    MessageBox MB_OK "Please enter a valid account name"
     Abort
  ${EndIf}

  # Check for empty password stored in R4
  !insertmacro MUI_INSTALLOPTIONS_READ $R4 ${PAGE_FILE} "Field 5" "State"
  ${If} $R4 == ""
    MessageBox MB_OK "Please enter a valid password"
     Abort
  ${EndIf}

  # Check for empty domain stored in R5
  !insertmacro MUI_INSTALLOPTIONS_READ $R5 ${PAGE_FILE} "Field 8" "State"
  ${If} $R5 == ""
    MessageBox MB_OK "Please enter a valid domain"
     Abort
  ${EndIf}

  #-----------------------------------------------------------------------------------
  # !! SELECTED: Specify an account !!
  #-----------------------------------------------------------------------------------

  # If "Specify an account" is selected check entered account
  !insertmacro MUI_INSTALLOPTIONS_READ $R0 ${PAGE_FILE} "Field 7" State
  ${If} $R0 == "1"
    # Check for service logon right
    UserMgr::HasPrivilege $R3 ${SERVICE_LOGON_RIGHT}
    Pop $0
    ${If} $0 == "TRUE"
      Goto checkGroupMember
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
      # Build the user environment of the user (Registry hive, Documents and settings etc.), returns status string
      UserMgr::BuiltAccountEnv $R3 $R4
      Pop $0
      ${If} $0 == "FALSE" # Means could not build account env
        DetailPrint "Unable to build account env"
        MessageBox MB_OK "Unable to build account env"
          Abort
      ${EndIf}
      # Here set R6 1 to know that a new account was created
      StrCpy $R6 "1"
    # Unknown result that means the account does not have the service logon right
    ${Else}
      DetailPrint "The account $R3 does not have the service logon right ! Result was $0"
      MessageBox MB_OK "The account $R3 does not have the log on service right assignment. In the 'Administrative Tools' of the 'Control Panel' open the 'Local Security Policy'. In 'Security Settings', select 'Local Policies' then select 'User Rights Assignments'. Finally, in the list of policies open the properties of 'Log on as a service' policy and add the account $R3."
        Abort # Go back to page.
    ${EndIf}
    checkGroupMember:
    # Check if the account is part of 'Performace Monitor Group' of SID "S-1-5-32-558"
    # The function UserMgr::IsMemberOfGroup does not work.
    # Instead we use the UserMgr::GetUserNameFromSID to check if the group exists if so
    # the user is added using a macro.
    UserMgr::GetUserNameFromSID ${PERFORMANCE_MONITOR_SID}
    Pop $0
    ${If} $0 == "ERROR"
      # The group does not exist, this occur on Windows XP so there is nothing to do
      Goto createServiceLABEL
    ${Else}
      # If a new account was created no need to ask to add the user to the group
      ${If} $R6 == "1"
        # The function UserMgr::AddToGroup does not work
        !insertmacro GetServerName $Hostname
        !insertmacro AddUserToGroup1 $Hostname $R3 "558"
      ${EndIf}
    ${EndIf}
    createServiceLABEL:
    # Create the service under the given use and password
    !insertmacro SERVICE "create" ${SERVICE_NAME} "path=$INSTDIR\ProActiveAgent.exe;autostart=1;interact=0;display=${SERVICE_NAME};description=${SERVICE_DESC};user=$R5\$R3;password=$R4;" ""
    Pop $0
    # Means the service is not installed !
    ${If} $0 != "true"
      DetailPrint "Unable to install as service."
      MessageBox MB_OK "Unable to install as service. To install manually use sc.exe command"
    ${EndIf}
  ${EndIf}

  writeToRegistryLABEL:
  ${If} $R7 == "1"
    # The goal is to read the path of AppData folder
    
    # Get the SID of the user
    UserMgr::GetSIDFromUserName $R5 $R3
    Pop $0
    #MessageBox MB_OK "User sid: $0"
    
    # On xp use the standard way
    ${If} ${IsWinXP}
      # If it is loaded in HKU by SID check in the Volatile Environment
      ReadRegStr $1 HKU "$0\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" AppData
      #MessageBox MB_OK "AppData path: $1"

      # If the result is empty, load the account into the HKU by username the read the same key
      ${If} $1 == ""
        UserMgr::RegLoadUserHive $R3
        #Pop $0
        #MessageBox MB_OK "Load ? $0"

        ReadRegStr $1 HKU "$R3\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" AppData
        #MessageBox MB_OK "User app data: $1"

        UserMgr::RegUnLoadUserHive $R3
        #Pop $0
        #MessageBox MB_OK "Load ? $0"
      ${EndIf}
    ${Else}
      # On other OS like Vista or 7 use HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList\SID ProfileImagePath
      # If it is loaded in HKU by SID check in the Volatile Environment
      ReadRegStr $1 HKLM "SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList\$0" ProfileImagePath
      StrCpy $1 "$1\AppData\Roaming"
    ${EndIf}
    
    #MessageBox MB_OK "AppData path: $1"

    StrCpy $R1 "$1\ProActiveAgent\config"
    # Copy the configuration file
    SetOutPath $R1
    File "utils\PAAgent-config.xml"
    # The location wil be written into the registry
    StrCpy $R1 "$R1\PAAgent-config.xml"
    # Create the logs dir
    CreateDirectory "$1\ProActiveAgent\logs"
    StrCpy $R2 "$1\ProActiveAgent\logs"
  ${EndIf}
  # Write the config file location into the registry
  WriteRegStr HKLM "Software\ProActiveAgent" "ConfigLocation" $R1
  # Write the config file location into the registry
  WriteRegStr HKLM "Software\ProActiveAgent" "LogsDirectory" $R2

  # If "Allow everyone to start/stop" is selected check for subinacl
  !insertmacro MUI_INSTALLOPTIONS_READ $R0 ${PAGE_FILE} "Field 15" State
  ${If} $R0 == "1"
    # Check if already installed
    IfFileExists "${SUBINACL_PATH}" existLABEL notExistLABEL
    notExistLABEL:
    # Ask the user if he wants to download the tool
    MessageBox MB_YESNO "To allow everyone to start/stop the agent a Microsoft Resource Kit is needed. Do you want to download it automatically from ${SUBINACL_URL} ? $\nNote that during the installation the default install path is required." IDYES downloadToolLABEL
      Abort
    downloadToolLABEL:
    # Automatic download of SubInAcl
    NSISdl::download ${SUBINACL_URL} $INSTDIR\SubInACL.msi
    # Check for downloaded msi file, if it does not exist report to user then finish installation
    IfFileExists "$INSTDIR\SubInACL.msi" downloadedLABEL problemLABEL
    problemLABEL:
    MessageBox MB_OK "Unable to download ${SUBINACL_URL} $\n${SUBINACL_MANUAL_INSTALL}"
    Goto runGuiLABEL
    downloadedLABEL:
    # Run the installer
    ExecWait 'cmd.exe /C "$INSTDIR\SubInACL.msi"'
    # Check if correctly installed
    IfFileExists "${SUBINACL_PATH}" existLABEL incorrectLABEL
    incorrectLABEL:
    MessageBox MB_OK "Unable to find ${SUBINACL_PATH} $\n${SUBINACL_MANUAL_INSTALL}"
    goto runGuiLABEL
    existLABEL:
    # Run the command in a console view to allow the user to see output
    # The first command allows control of the service by ALL USERS group
    # The second command allows full control of the configuration file by ALL USERS group
    ExecWait 'cmd.exe /C cd "${SUBINACL_DIR}" & subinacl.exe /service ${SERVICE_NAME} /grant=S-1-1-0=TO & subinacl.exe /file "$R1" /grant=S-1-1-0=F & pause'
  ${EndIf}
  runGuiLABEL:
  # Run the Agent GUI
  Exec "$INSTDIR\AgentForAgent.exe"
FunctionEnd

#############################
# !! SECTIONS DEFINITIONS !!
#############################

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
        # Copy the GUI
        #-----------------------------------------------------------------------------------
        SetOutPath $INSTDIR
        File "bin\Release\AgentForAgent.exe"
SectionEnd

Section "ProActive ScreenSaver"
        SetOutPath $SYSDIR
        File "bin\Release\ProActiveSSaver.scr"
SectionEnd

######################################
# Section "Desktop shortcuts"
#         SetShellVarContext all # All users
#         IfFileExists $INSTDIR\AgentForAgent.exe 0 +2
#           CreateShortCut "$DESKTOP\ProActive Agent Control.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\icon.ico" 0
#         SetShellVarContext current # Current User
# SectionEnd
######################################

Section "Start Menu Shortcuts"
        SetShellVarContext all # All users
        CreateDirectory "$SMPROGRAMS\ProActiveAgent"
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Control.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\icon.ico" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\Uninstall ProActive Agent.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Documentation.lnk" "$INSTDIR\ProActive Agent Documentation.pdf" "" "$INSTDIR\ProActive Agent Documentation.pdf" 0
        SetShellVarContext current # reset to current user
SectionEnd

#uninstall section

UninstallText "This will uninstall ProActive Agent. Hit next to continue."

Section "Uninstall"
        #-----------------------------------------------------------------------------------
        # Check user admin rights
        #-----------------------------------------------------------------------------------
        System::Call "kernel32::GetModuleHandle(t 'shell32.dll') i .s"
        System::Call "kernel32::GetProcAddress(i s, i 680) i .r0"
        System::Call "::$0() i .r0"
        DetailPrint "Check: Current user is admin? $0"
        StrCmp $0 '0' 0 +3
          MessageBox MB_OK "Adminstrator rights are required to uninstall the ProActive Agent."
          Abort


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
        
	#-----------------------------------------------------------------------------------
        # Close the ProActive Agent Control
        #-----------------------------------------------------------------------------------
        Push ""
        Push "ProActive Agent Control"
        Call un.FindWindowClose
       
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
        Delete "SubInACL.msi"

	RMDir /r "$SMPROGRAMS\ProActiveAgent"
	SetShellVarContext current # reset to current user
	
SectionEnd

Function un.FindWindowClose
    Exch $0
    Exch
    Exch $1
    Push $2
    Push $3
    find:
        FindWindow $2 $1 $0
        IntCmp $2 0 nowindow
        SendMessage $2 16 "" ""
        Sleep 500
        FindWindow $2 $1 $0
        IntCmp $2 0 nowindow
            MessageBox MB_OK|MB_ICONSTOP "An instance of the program is running. Please close it and press OK to continue."
            Goto find
    nowindow:
    Pop $3
    Pop $2
    Pop $1
    Pop $0
FunctionEnd