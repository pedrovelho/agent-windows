############################################################################################################################
# Includes:
# - x64.nsh for few simple macros to handle installation on x64 architecture
# - FileFunc.nsh for manipulating options file
# - DotNetVer.nsh for checking Microsoft .NET Framework versions (see http://ontheperiphery.veraida.com/project/dotnetver)
# - servicelib.nsh for Windows service installation
# - UserManagement.nsh contains GetServerName and AddUserToGroup1 from http://nsis.sourceforge.net/User_Management_using_API_calls#Add_User_to_a_group
############################################################################################################################
!include x64.nsh
!include MUI.nsh
!include WinVer.nsh
!include FileFunc.nsh
!include "DotNetVer.nsh"
!include "servicelib.nsh"
!include "UserManagement.nsh"

#################################################################
# !! TARGET_ARCH !! can be x86 or x64
#################################################################
!define TARGET_ARCH "x86"

#################################################################
# Some constants definitions like service name, version, etc ...
#################################################################
!define SERVICE_NAME "ProActiveAgent"
!define SERVICE_DESC "The ProActive Agent enables desktop computers as an important source of computational power"
!define VERSION "2.4"
!define PAGE_FILE "serviceInstallPage.ini"
!define INSTALL_LOG "$INSTDIR\install.log"

#################################################################
# Privileges required by the ProActive Runtime Account
#################################################################
!define SERVICE_LOGON_RIGHT 'SeServiceLogonRight'
!define SE_INCREASE_QUOTA_NAME 'SeIncreaseQuotaPrivilege'
!define SE_ASSIGNPRIMARYTOKEN_NAME 'SeAssignPrimaryTokenPrivilege'

!define CONFIG_NAME "PAAgent-config.xml"
!define DEFAULT_CONFIG_PATH "$INSTDIR\config\${CONFIG_NAME}"
!define PERFORMANCE_MONITOR_SID "S-1-5-32-558"
!define TXT_CONF "Field 9"
!define TXT_LOGSDIR "Field 10"
!define CHK_LOGSHOME "Field 12"
!define CHK_ALLOWANY "Field 11"
!define TXT_DOMAIN "Field 15"

# Fixed parameters for TerminateAgentForAgent function
!define WND_TITLE "ProActive Agent Control"
!define TO_MS 2000
!define SYNC_TERM 0x0010000

Var Hostname
Var tmp
Var uninstall
Var overrideConfig

CRCCheck on

Name "ProActive Agent ${VERSION}"
OutFile ProActiveAgent-${VERSION}-setup.exe

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
Page Custom ConfigureSetupPage HandleSetupArguments

##########################################################################################################################################
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

##########################################################################################################################################
# The following macro performs a user login, the DoLogonUser macro must be called like the following:
# !insertmacro DoLogonUser "domain" "username" "password"
# # Store user token
# StrCpy $8 $R0
# # Logoff user
# !insertmacro DoLogoffUser $8
#####################################################################

!ifndef LogonUser
!define LogonUser "AdvAPI32::LogonUserW(w, w, w, i, i, *i) i"
!endif
!ifndef CloseHandle
!define CloseHandle "Kernel32::CloseHandle(i) i"
!endif
!ifndef LOGON32_LOGON_NETWORK
!define LOGON32_LOGON_NETWORK 3
!endif
!ifndef LOGON32_PROVIDER_DEFAULT
!define LOGON32_PROVIDER_DEFAULT 0
!endif

!ifmacrondef DoLogonUser
  ;Logs on a user, and returns their login token in $R0
  !macro DoLogonUser Domain Username Password
    System::Call "${LogonUser}('${Username}', '${Domain}', '${Password}', ${LOGON32_LOGON_NETWORK}, ${LOGON32_PROVIDER_DEFAULT}, .R0) .R8"
    StrCmp $R8 0 logOnErr
    goto logOnDone
    logOnErr:
      MessageBox MB_OK "Invalid username or password"
    logOnDone:
      DetailPrint "Return token: $R0"
      DetailPrint "Call return: $R8"
  !macroend
!endif

!ifmacrondef DoLogoffUser
  ;Logs off a user token (Returned from DoLogonUser)
  !macro DoLogoffUser Token
    System::Call "${CloseHandle}(${Token}) .R5"
    StrCmp $R5 0 logOffErr
    goto logOffDone
    logOffErr:
      DetailPrint "Error: Logging user off (Token: '${Token}')"
    logOffDone:
      DetailPrint "Close return: $R5"
  !macroend
!endif

#####################################################################
# Logs into the detailed gui section and into the log file
#####################################################################
!macro Log str
  DetailPrint "${str}"
  nsislog::log "${INSTALL_LOG}" "${str}"
!macroend

#####################################################################
# Runs uninstaller if available
#####################################################################
Function RollbackIfSilent
  ${If} ${Silent}
    !insertmacro Log "Rollbacking  uninstaller ..."
    ${If} ${FileExists} '$INSTDIR\uninstall.exe'
      ExecWait '"$INSTDIR\uninstall.exe" /S ?_=$INSTDIR'
    ${EndIf}
  ${EndIf}
FunctionEnd

##########################################################################################################################################
# On init peforms the following checks:
# - admin rights
# - Microsoft .NET Framework 3.5
# - Previous version of the unistaller
########################################
Function .onInit
    # Read hostname
  !insertmacro GetServerName $Hostname
  
  ${If} ${Silent}
    ${GetParameters} $0
    ${If} $0 != ""
      # Run the uninstaller if /UN is specified.
      ${GetOptions} "$0" "/UN" $tmp
      IfErrors continue_install
        Call RollbackIfSilent
        Abort
      continue_install:
        ClearErrors
        ${GetOptions} "$0" "/A" $tmp
        IfErrors +2
        StrCpy $R0 "1"
        ClearErrors
        ${GetOptions} "$0" "/H" $tmp
        IfErrors +2
        StrCpy $R7 "1"
        ClearErrors
        ${GetOptions} "$0" "/O" $tmp
        StrCpy $overrideConfig "1"
        # Read and store command-line arguments
        ${GetOptions} "$0" "/CONFIG_DIR=" $R1
        # If not specified set default value
        ${If} $R1 == ""
           StrCpy $R1 "$INSTDIR\config"
        ${EndIf}
        ${GetOptions} "$0" "/LOG_DIR=" $R2
        # If not specified set default value
        ${If} $R2 == ""
           StrCpy $R2 "$INSTDIR\logs"
        ${EndIf}
        ${GetOptions} "$0" "/USER=" $R3
        ${GetOptions} "$0" "/PASSWORD=" $R4
        ${GetOptions} "$0" "/DOMAIN=" $R5
        # If not specified set hostname as value
        ${If} $R5 == ""
           StrCpy $R5 $Hostname
        ${EndIf}
        ClearErrors
        ${GetOptions} "$0" "/INI=" $1
        ${Unless} ${Errors}
          # If the requried parameter is not given as a command-line argument,
          # check whether it is specified in a configuration file (if any)
          ${If} ${FileExists} "$1"
            # THIS will override the  commandline argument
            ReadINIStr $2 $1 "INSTALL" "INST_DIR"
            # Set the INSTDIR variable if a installation directory specified
            ${If} $2 != ""
              StrCpy $INSTDIR $2
            ${EndIf}
            ${If} $R1 == ""
              ReadINIStr $R1 $1 "INSTALL" "CONFIG_DIR"
            ${EndIf}
            ${If} $R2 == ""
              ReadINIStr $R2 $1 "INSTALL" "LOG_DIR"
            ${EndIf}
            ${If} $R3 == ""
              ReadINIStr $R3 $1 "INSTALL" "USER"
            ${EndIf}
            ${If} $R4 == ""
              ReadINIStr $R4 $1 "INSTALL" "PASSWORD"
            ${EndIf}
            ${If} $R5 == ""
              ReadINIStr $R5 $1 "INSTALL" "DOMAIN"
            ${EndIf}
            ${If} $R0 == ""
              ReadINIStr $R0 $1 "INSTALL" "ALLOW_ALL_USERS_TO_START_STOP"
              # Allow all users to start/stop
              ${If} $R0 == ""
                # default
                StrCpy $R0 "0"
              ${EndIf}
            ${EndIf}
            ${If} $R7 == ""
              ReadINIStr $R7 $1 "INSTALL" "USE_SERVICE_ACCOUNT_HOME"
              ${If} $R7 == ""
                # default
                StrCpy $R7 "0"
              ${EndIf}
            ${EndIf}
            ${If} $overrideConfig == ""
              ReadINIStr $overrideConfig $1 "INSTALL" "OVERRIDE_CONFIG"
              ${If} $overrideConfig == ""
                # default
                StrCpy $overrideConfig "0"
              ${EndIf}
            ${EndIf}
          ${EndIf}
        ${EndUnless}
    ${EndIf}
  ${EndIf}
  # Check user admin rights
  System::Call "kernel32::GetModuleHandle(t 'shell32.dll') i .s"
  System::Call "kernel32::GetProcAddress(i s, i 680) i .r0"
  System::Call "::$0() i .r0"
  !insertmacro Log "Current user is admin? $0"
  StrCmp $0 '0' 0 +3
  MessageBox MB_OK "Adminstrator rights are required to install the ProActive Agent." /SD IDOK
  Abort

  #Check if .NET framework 3.5 is installed
  ${IfNot} ${HasDotNet3.5}
    !insertmacro Log "!! Unable to find Microsoft .NET Framework 3.5 !!"
    MessageBox MB_OK "Unable to find Microsoft .NET Framework 3.5" /SD IDOK
    Abort
  ${EndIf}

  # Check if a previous version of the unistaller is available
  IfFileExists $INSTDIR\uninstall.exe 0 endLabel
  # Ask the user if he wants to uninstall previous version
  ${If} ${Silent}
    ${If} $uninstall == 1
      ExecWait '"$INSTDIR\uninstall.exe /S" _?=$INSTDIR'
    ${EndIf}
  ${Else}
    !insertmacro Log "Uninstalling previous version of the ProActive Agent ..."
    MessageBox MB_YESNO "The previous version of the ProActive Windows Agent must be uninstalled. Run the uninstaller ?" /SD IDYES IDNO abortLabel
    ExecWait '"$INSTDIR\uninstall.exe" _?=$INSTDIR'
  ${EndIf}
  Goto endLabel

  abortLabel:
    Abort

  endLabel:
     # In silent mode, we needs to explicitly handle parameters and installation
    ${If} ${Silent}
      !insertmacro Log "Running installer in silent mode ..."
      Call InstallProActiveAgent
      Call InstallProActiveScreenSaver
      Call CreateDesktopShortCuts
      Call ProcessSetupArguments
    ${EndIf}
FunctionEnd

Function ConfigureSetupPage
  ReserveFile ${PAGE_FILE}
  !insertmacro MUI_INSTALLOPTIONS_EXTRACT ${PAGE_FILE}
  # Set default location for configuration file
  !insertmacro MUI_INSTALLOPTIONS_WRITE ${PAGE_FILE} "${TXT_CONF}" State "$INSTDIR\config"
  # Set default location for logs directory
  !insertmacro MUI_INSTALLOPTIONS_WRITE ${PAGE_FILE} "${TXT_LOGSDIR}" State "$INSTDIR\logs"
  # Set default user domain
  !insertmacro MUI_INSTALLOPTIONS_WRITE ${PAGE_FILE} "${TXT_DOMAIN}" State $Hostname
  # Disable "Use service account home"
  # !insertmacro GROUPCONTROLS "${PAGE_FILE}" "${SEL_INSTLOC}" "${CHK_LOGSHOME}|" "1"
  # Display the custom page
  !insertmacro MUI_INSTALLOPTIONS_DISPLAY ${PAGE_FILE}
FunctionEnd

Function HandleSetupArguments
  Call ReadSetupArguments
  Call ProcessSetupArguments
FunctionEnd

Function ReadSetupArguments
  # Handle notify event of checkbox "Use service account home"
  !insertmacro MUI_INSTALLOPTIONS_READ $tmp "${PAGE_FILE}" "Settings" "State"
  ${Switch} "Field $tmp"
    ${Case} "${CHK_LOGSHOME}"
      !insertmacro GROUPCONTROLS "${PAGE_FILE}" "${CHK_LOGSHOME}" "${TXT_LOGSDIR}|${TXT_CONF}" "1"
      Abort
  ${EndSwitch}

  # CHECK LOCATIONS

  # Check "Use service account home" for logs location stored in R7
  !insertmacro MUI_INSTALLOPTIONS_READ $R7 ${PAGE_FILE} "${CHK_LOGSHOME}" State
  ${If} $R7 == "1"
    Goto skipLocationsLABEL
  ${EndIf}
  !insertmacro MUI_INSTALLOPTIONS_READ $R1 ${PAGE_FILE} "${TXT_CONF}" State
  !insertmacro MUI_INSTALLOPTIONS_READ $R2 ${PAGE_FILE} "${TXT_LOGSDIR}" State

  skipLocationsLABEL:
  # CHECK ACCOUNT FIELDS
  !insertmacro MUI_INSTALLOPTIONS_READ $R3 ${PAGE_FILE} "Field 4" "State"
  !insertmacro MUI_INSTALLOPTIONS_READ $R4 ${PAGE_FILE} "Field 5" "State"
  !insertmacro MUI_INSTALLOPTIONS_READ $R5 ${PAGE_FILE} "${TXT_DOMAIN}" "State"
  !insertmacro MUI_INSTALLOPTIONS_READ $R0 ${PAGE_FILE} "${CHK_ALLOWANY}" State
FunctionEnd

Function ProcessSetupArguments
  ${If} $R7 == "1"
    Goto skipLocationsLABEL
  ${EndIf}

  # Check config file location stored in R1
  ${If} ${Silent}
    # In silent mode set '$INSTDIR/config' as the default config directory if not specified
    ${If} $R1 == ""
       StrCpy $R1 "$INSTDIR\config"
    ${EndIf}
  ${Else}
      !insertmacro MUI_INSTALLOPTIONS_READ $R1 ${PAGE_FILE} "${TXT_CONF}" State
  ${EndIf}
  ${If} $R1 == ""
    !insertmacro Log "!! Invalid config file location !!"
    Call RollbackIfSilent
    MessageBox MB_OK "Please enter a valid location for the Configuration File" /SD IDOK
     Abort
  ${Else}
    # If the location is NOT THE DEFAULT ONE
    ${If} $R1 != "$INSTDIR\config"
      # Check if the specified directory exists
      IfFileExists $R1 dirExistLABEL dirNotExistLABEL
      dirNotExistLABEL:
      MessageBox MB_OK "The specified directory $R1 for configuration file doesn't exist" /SD IDOK
        Abort
      dirExistLABEL:
      # Check if there is already a config file
      IfFileExists "$R1\${CONFIG_NAME}" askUseLABEL copyDefaultLABEL
      askUseLABEL:
      # Ask the user if he wants to use the existing file (if not the default one will be copied to this dir)
      MessageBox MB_YESNO "Use existing configuration file $R1\${CONFIG_NAME} ?" /SD IDYES IDYES setLocationLABEL
      copyDefaultLABEL:
      SetOutPath $R1
      File "utils\PAAgent-config.xml"
      setLocationLABEL:
      !insertmacro Log "Setting existing location ..."
      StrCpy $R1 "$R1\${CONFIG_NAME}" # R1 will contain the full path
    ${Else}
      StrCpy $R1 "$INSTDIR\config\${CONFIG_NAME}"
    ${EndIf}
  ${EndIf}

  # Check logs location stored in R2
  !insertmacro Log "Checking logs location ..."
  ${If} ${Silent}
     # Set '$INSTDIR/logs' as the default log directory if not specified
     ${If} $R2 == ""
        StrCpy $R2 "$INSTDIR\logs"
     ${EndIf}
  ${Else}
    !insertmacro MUI_INSTALLOPTIONS_READ $R2 ${PAGE_FILE} "${TXT_LOGSDIR}" State
  ${EndIf}
  ${If} $R2 == ""
    !insertmacro Log "!! Invalid logs location !!"
    Call RollbackIfSilent
    MessageBox MB_OK "Please enter a valid location for the logs" /SD IDOK
     Abort
  ${EndIf}

  skipLocationsLABEL:

  #-----------------------------------------------------------------------------------
  # !! CHECK ACCOUNT FIELDS !!
  #-----------------------------------------------------------------------------------

  # Check for empty username stored in R3
  ${IfNot} ${Silent}
    !insertmacro MUI_INSTALLOPTIONS_READ $R3 ${PAGE_FILE} "Field 4" "State"
  ${EndIf}
  ${If} $R3 == ""
    !insertmacro Log "!! Invalid account name !!"
    Call RollbackIfSilent
    MessageBox MB_OK "Please enter a valid account name" /SD IDOK
     Abort
  ${EndIf}

  # Check for empty password stored in R4
  ${IfNot} ${Silent}
    !insertmacro MUI_INSTALLOPTIONS_READ $R4 ${PAGE_FILE} "Field 5" "State"
  ${EndIf}
  ${If} $R4 == ""
    !insertmacro Log "!! Invalid password !!"
    Call RollbackIfSilent
    MessageBox MB_OK "Please enter a valid password" /SD IDOK
     Abort
  ${EndIf}

  # Check for empty domain stored in R5
  ${If} ${Silent}
     # Set hostname as the default domain
     ${If} $R5 == ""
        !insertmacro GetServerName $R5
     ${EndIf}
  ${Else}
     !insertmacro MUI_INSTALLOPTIONS_READ $R5 ${PAGE_FILE} "${TXT_DOMAIN}" "State"
  ${EndIf}
  ${If} $R5 == ""
    !insertmacro Log "!! Invalid domain !!"
    Call RollbackIfSilent
    MessageBox MB_OK "Please enter a valid domain" /SD IDOK
     Abort
  ${EndIf}

  #-----------------------------------------------------------------------------------
  # !! SELECTED: Specify an account !!
  #-----------------------------------------------------------------------------------

  checkAccount:
  # Try to log under the specified account
  !insertmacro DoLogonUser $R5 $R3 $R4
  StrCmp $R8 0 unableToLog
  Goto logged
  # If unable to log ask if the admin wants to create a local account for the agent
  unableToLog:
  Goto createNewAccount
  logged:

  # The user is logged it means the password is correct
  !insertmacro Log "Sucessfully logged on as $R3, logging off ..."
  MessageBox MB_OK "Sucessfully logged on as $R3, logging off" /SD IDOK

    # Logoff user
    StrCpy $0 $R5
    !insertmacro DoLogoffUser $R0
    StrCpy $R5 $0

    # Checking privileges required for RunAsMe mode
    UserMgr::HasPrivilege $R3 ${SE_INCREASE_QUOTA_NAME}
    Pop $0
    !insertmacro Log "Does $R3 have the privilege     SE_INCREASE_QUOTA_NAME ? UserMgr::HasPrivilege returns $0 ..."
    ${If} $0 != "TRUE"
        MessageBox MB_OK|MB_ICONSTOP "In order to use RunAsMe mode, the account $R3 must have 'Adjust memory quotas for a process' and 'Replace a process-level token' privileges. In the 'Administrative Tools' of the 'Control Panel' open the 'Local Security Policy'. In 'Security Settings', select 'Local Policies' then select 'User Rights Assignments'. Finally, in the list of policies open the corresponding properties and add the account $R3." /SD IDOK
    ${EndIf}

    UserMgr::HasPrivilege $R3 ${SE_ASSIGNPRIMARYTOKEN_NAME}
    Pop $0
    !insertmacro Log "Does $R3 have the privilege SE_ASSIGNPRIMARYTOKEN_NAME ? UserMgr::HasPrivilege returns $0 ..."
    ${If} $0 != "TRUE"
        MessageBox MB_OK|MB_ICONSTOP "In order to use RunAsMe mode, the account $R3 must have 'Adjust memory quotas for a process' and 'Replace a process-level token' privileges. In the 'Administrative Tools' of the 'Control Panel' open the 'Local Security Policy'. In 'Security Settings', select 'Local Policies' then select 'User Rights Assignments'. Finally, in the list of policies open the corresponding properties and add the account $R3." /SD IDOK
    ${EndIf}

    Goto checkGroupMember

    # The account does not exist if the specified domain is local computer the account can be created
    createNewAccount:
      ${If} $R5 == $Hostname
        !insertmacro Log "The account $R3 does not exist, asking user if he wants to create a new account (not created in silent mode)"
         # Ask the user if he wants to create a new account
         MessageBox MB_YESNO "The account $R3 does not exist, would you like to create it ?" /SD IDNO IDYES createAccount
           Abort
      ${Else}
         # The  domain is not local so the account cannot be created
         MessageBox MB_OK "The account $R3 does not exist, since the Domain is not local the account cannot be created." /SD IDOK
           Abort
      ${EndIf}
      DetailPrint "The account $R3 does not exist ... asking user if he wants to create a new account"
      # Ask the user if he wants to create a new account
      MessageBox MB_YESNO "The account $R3 does not exist, would you like to create it ?" /SD IDYES IDYES createAccount
        Abort
      createAccount:
      !insertmacro Log "Creating $R3 account locally ..."
      UserMgr::CreateAccount $R3 $R4 "The ProActive Agent runs a Java Virtual Machine under this account."
      Pop $0
      ${If} $0 == "ERROR 2224" # Means account already exists .. it's strange but yes it is possible !
        !insertmacro Log "!! The account $R3 already exist !!"
        MessageBox MB_OK "The account $R3 already exist" /SD IDOK
          Abort
      ${ElseIf} $0 == "ERROR 2245" # The password requirements are not met (too short)
        !insertmacro Log "!! Password requirements are not met !!"
        MessageBox MB_OK "The password does not meet the password policy requirements. Check the minimum password length, password complexity and password history requirements." /SD IDOK
          Abort
      ${Else}
        ${If} $0 != "OK"
          MessageBox MB_OK "Unable to create the service. ERROR $0" /SD IDOK
           Abort
        ${EndIf}
      ${EndIf}
      # Add SE_INCREASE_QUOTA_NAME account privilege
      UserMgr::AddPrivilege $R3 ${SE_INCREASE_QUOTA_NAME}
      Pop $0
      ${If} $0 == "FALSE" # Means could not add privilege
        !insertmacro Log "!! Unable to add privilege SE_INCREASE_QUOTA_NAME !!"
        MessageBox MB_OK "Unable to add privilege SE_INCREASE_QUOTA_NAME" /SD IDOK
          Abort
      ${EndIf}
      # Add SE_ASSIGNPRIMARYTOKEN_NAME account privilege
      UserMgr::AddPrivilege $R3 ${SE_ASSIGNPRIMARYTOKEN_NAME}
      Pop $0
      ${If} $0 == "FALSE" # Means could not add privilege
        !insertmacro Log "!! Unable to add privilege SE_ASSIGNPRIMARYTOKEN_NAME !!"
        MessageBox MB_OK "Unable to add privilege SE_ASSIGNPRIMARYTOKEN_NAME" /SD IDOK
          Abort
      ${EndIf}
      # Build the user environment of the user (Registry hive, Documents and settings etc.), returns status string
      UserMgr::BuiltAccountEnv $R3 $R4
      Pop $0
      ${If} $0 == "FALSE" # Means could not build account env
        !insertmacro Log "!! Unable to build account env !!"
        Call RollbackIfSilent
        MessageBox MB_OK "Unable to build account env" /SD IDOK
          Abort
      ${EndIf}
      # Here set R6 1 to know that a new account was created
      StrCpy $R6 "1"
      Goto checkAccount

    checkGroupMember:

    # Check if the account is part of 'Performace Monitor Group' of SID "S-1-5-32-558"
    # The function UserMgr::IsMemberOfGroup does not work.
    # Instead we use the UserMgr::GetUserNameFromSID to check if the group exists if so
    # the user is added using a macro.
    UserMgr::GetUserNameFromSID ${PERFORMANCE_MONITOR_SID}
    Pop $0
    ${If} $0 == "ERROR"
      # The group does not exist, this occur on Windows XP so there is nothing to do
      ${IfNot} ${IsWinXP}
         !insertmacro Log "!! ${PERFORMANCE_MONITOR_SID} does not exist !!"
      ${EndIf}
      Goto createServiceLABEL
    ${Else}
      # If a new account was created no need to ask to add the user to the group
      ${If} $R6 == "1"
        # The function UserMgr::AddToGroup does not work
        !insertmacro AddUserToGroup1 $Hostname $R3 "558"
      ${EndIf}
    ${EndIf}

    createServiceLABEL:

    !insertmacro Log "Installing ProActiveAgent.exe as service ..."
    # Create the service under the Local System and store the user and password in the restricted registry key
    ${If} ${Silent}
      nsExec::Exec 'cmd.exe /C sc create ${SERVICE_NAME} binPath= "$INSTDIR\ProActiveAgent.exe" DisplayName= "${SERVICE_NAME}" start= auto type= interact type= own & sc description ${SERVICE_NAME} "${SERVICE_DESC}"'
      # !! OMIT THIS MESSAGE CHECK SERVICE INSTALL BY QUERYING THE SERVICE STATUS !!
      #!insertmacro Log "Is ok ? $0"
    ${Else}
      ExecWait 'cmd.exe /C echo Installing "$INSTDIR\ProActiveAgent.exe" as service ... & sc create ${SERVICE_NAME} binPath= "$INSTDIR\ProActiveAgent.exe" DisplayName= "${SERVICE_NAME}" start= auto type= interact type= own & sc description ${SERVICE_NAME} "${SERVICE_DESC}" & pause'
    ${EndIf}

    # Check if the service was correctly installed
    !insertmacro Log "Checking service installation ..."
    !insertmacro SERVICE "status" ${SERVICE_NAME} '' ""
    Pop $0
    ${If} $0 != "stopped"
      !insertmacro Log "!! Unable to install as service !!"
      Call RollbackIfSilent
      MessageBox MB_OK "Unable to install as service. To install manually use sc.exe command" /SD IDOK
       Abort
    ${EndIf}

    # Once the service is installed write auth data into a restricted acces key
    WriteRegStr HKLM "Software\ProActiveAgent\Creds" "domain" $R5
    WriteRegStr HKLM "Software\ProActiveAgent\Creds" "username" $R3

    #######################################
    # Encrypt the password, see AGENT-154 #
    #######################################
    !insertmacro Log "Encrypting password ..."
    # Set current dir to the location of pacrypt.dll
    SetOutPath $INSTDIR
    # C-Signature: int encryptData(wchar_t *input, wchar_t *output)
    StrCpy $0 $R4 # copy register to stack
    System::Call "pacrypt::encryptData(w, w) i(r0., .r1).r2"
    ${If} $2 != 0
      !insertmacro Log "!! Unable to encrypt the password (too long ?). Error $2 !!"
      Call RollbackIfSilent
      MessageBox MB_OK "Unable to encrypt the password (too long ?). Error $2" /SD IDOK
      Abort
    ${EndIf}
    # Uncomment the following code to chek if the encryption mechanism works correctly
    # the last message box should show
    #MessageBox MB_OK "---> $0 , $1 , $2"
    #System::Call "pacrypt::decryptData(w, w) i(r1., .r4).r0"
    #MessageBox MB_OK "---> $0 , $1 , $4"

    # Write encrypted password in registry
    WriteRegStr HKLM "Software\ProActiveAgent\Creds" "password" $1

    UserMgr::GetSIDFromUserName $R5 $R3
    Pop $0

    !insertmacro Log "Restricting keyfile and regkey access ..."
    # Run the command in a console view to allow the user to see output
    # The command based on SID grants full permissions only for LocalSystem and Administrators to the keyfile and the registry key
    ${If} ${Silent}
      nsExec::Exec '"$INSTDIR\SetACL.exe" -on "$INSTDIR\restrict.dat" -ot file -actn setprot -op "dacl:p_nc;sacl:p_nc" -actn ace -ace "n:S-1-5-18;p:full;s:y" -ace "n:S-1-5-32-544;p:full;s:y"'
      ${If} $0 == "ERROR"
        !insertmacro Log "!! Unable to restrict keyfile access !!"
        Call RollbackIfSilent
        Abort
      ${EndIf}
      nsExec::Exec '"$INSTDIR\SetACL.exe" -on HKEY_LOCAL_MACHINE\Software\ProActiveAgent\Creds -ot reg -actn setprot -op "dacl:p_nc;sacl:p_nc" -actn ace -ace "n:S-1-5-18;p:full;s:y" -ace "n:S-1-5-32-544;p:full;s:y"'
      ${If} $0 == "ERROR"
        !insertmacro Log "!! Unable to restrict regkey access !!"
        Call RollbackIfSilent
        Abort
      ${EndIf}
    ${Else}
      ExecWait 'cmd.exe /C \
        echo Restricting keyfile access ... &\
        "$INSTDIR\SetACL.exe" -on "$INSTDIR\restrict.dat" -ot file -actn setprot -op "dacl:p_nc;sacl:p_nc" -actn ace -ace "n:S-1-5-18;p:full;s:y" -ace "n:S-1-5-32-544;p:full;s:y"&\
        echo Restricting regkey access ... &\
        "$INSTDIR\SetACL.exe" -on HKEY_LOCAL_MACHINE\Software\ProActiveAgent\Creds -ot reg -actn setprot -op "dacl:p_nc;sacl:p_nc" -actn ace -ace "n:S-1-5-18;p:full;s:y" -ace "n:S-1-5-32-544;p:full;s:y"&\
        pause'
    ${EndIf}

  ${If} $R7 == "1"
    # The goal is to read the path of AppData folder

    # Get the SID of the user
    UserMgr::GetSIDFromUserName "." $R3
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

  # If "Allow everyone to start/stop" is selected
  !insertmacro MUI_INSTALLOPTIONS_READ $R0 ${PAGE_FILE} "${CHK_ALLOWANY}" State
  ${If} $R0 == "1"
    !insertmacro Log "Allowing everyone to control the service ..."

    # Run the command in a console view to allow the user to see output
    # The first command allows control of the service by ALL USERS group
    # The second command allows full control of the configuration file by ALL USERS group
    ${If} ${Silent}
      nsExec::Exec '"$INSTDIR\SetACL.exe" -on ${SERVICE_NAME} -ot srv -actn ace -ace "n:S-1-1-0;p:start_stop;s:y"'
      ${If} $0 == "ERROR"
        !insertmacro Log "!! Unable to allow control of the service !!"
        Call RollbackIfSilent
        Abort
      ${EndIf}
      nsExec::Exec '"$INSTDIR\SetACL.exe" -on "$R1" -ot file -actn ace -ace "n:S-1-1-0;p:full;s:y"'
      ${If} $0 == "ERROR"
        !insertmacro Log "!! Unable to allow control of the config file !!"
        Call RollbackIfSilent
        Abort
      ${EndIf}
    ${Else}
      ExecWait 'cmd.exe /C \
        echo Granting start/stop permissions on ${SERVICE_NAME} service to everyone ... &\
        "$INSTDIR\SetACL.exe" -on ${SERVICE_NAME} -ot srv -actn ace -ace "n:S-1-1-0;p:start_stop;s:y"&\
        echo Granting full access on the configuration file to everyone ... &\
        "$INSTDIR\SetACL.exe" -on "$R1" -ot file -actn ace -ace "n:S-1-1-0;p:full;s:y"&\
        pause'
    ${EndIf}
  ${EndIf}

  !insertmacro Log "Ready to start the ProActiveAgent service ..."
  ${If} ${Silent}
    # If silent mode just exit
    Abort
  ${Else}
    # Run the Agent GUI
    Exec "$INSTDIR\AgentForAgent.exe"
  ${EndIf}
FunctionEnd

#############################
# !! SECTIONS DEFINITIONS !!
#############################

Section "ProActive Agent"
  Call InstallProActiveAgent
SectionEnd

Section "ProActive ScreenSaver"
  Call InstallProActiveScreenSaver
SectionEnd

Section "Start Menu Shortcuts"
  Call CreateDesktopShortCuts
SectionEnd

Function InstallProActiveAgent
        #-----------------------------------------------------------------------------------
        # Print the current date and time into the installation log file
        #-----------------------------------------------------------------------------------
        Call GetCurrentDate
        Pop $R0
        Call GetCurrentTime
        Pop $R1
        !insertmacro Log "$R0 - $R1 Installing ProActiveAgent v${VERSION} ..."
        StrCpy $R0 ""
        StrCpy $R1 ""

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
        File "bin\Release\ProActiveAgent.exe"
        File "bin\Release\parunas.exe"
        File "bin\Release\pacrypt.dll"
        File "utils\icon.ico"
        File "utils\ListNetworkInterfaces.class"
        File "utils\delete_temp.bat"
        File "utils\SetACL.exe"
        File "ProActiveAgent\log4net.config"
        File "ProActiveAgent\lib\log4net.dll"
        File "ProActiveAgent\lib\InJobProcessCreator.exe"
        File "ProActiveAgent\lib\${TARGET_ARCH}\JobManagement.dll"

        IfFileExists $INSTDIR\config\PAAgent-config.xml 0 defaultFileNotExistLabel

        MessageBox MB_YESNO "Use existing configuration file $INSTDIR\config\PAAgent-config.xml ?" /SD IDYES IDNO defaultFileNotExistLabel
        !insertmacro Log "Using existing configuration file ..."
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
        SetOutPath $INSTDIR\doc
        File "ProActive Agent Documentation.pdf"

        #-----------------------------------------------------------------------------------
        # The agent requires the following reg sub-key to know its default configuration
        #-----------------------------------------------------------------------------------
        WriteRegStr HKLM "Software\ProActiveAgent" "ConfigLocation" "$INSTDIR\config\PAAgent-config.xml"

        #-----------------------------------------------------------------------------------
        # Copy the GUI
        #-----------------------------------------------------------------------------------
        SetOutPath $INSTDIR
        File "bin\Release\AgentForAgent.exe"
        
        !insertmacro Log "Successfully copied files ..."
FunctionEnd

Function InstallProActiveScreenSaver
        SetOutPath $SYSDIR
        File "bin\Release\ProActiveSSaver.scr"
FunctionEnd

######################################
# Section "Desktop shortcuts"
#         SetShellVarContext all # All users
#         IfFileExists $INSTDIR\AgentForAgent.exe 0 +2
#           CreateShortCut "$DESKTOP\ProActive Agent Control.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\icon.ico" 0
#         SetShellVarContext current # Current User
# SectionEnd
######################################

Function CreateDesktopShortCuts
        SetShellVarContext all # All users
        CreateDirectory "$SMPROGRAMS\ProActiveAgent"
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Control.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\icon.ico" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\Uninstall ProActive Agent.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Documentation.lnk" "$INSTDIR\doc\ProActive Agent Documentation.pdf" "" "$INSTDIR\doc\ProActive Agent Documentation.pdf" 0
        SetShellVarContext current # reset to current user
FunctionEnd

#uninstall section

UninstallText "This will uninstall ProActive Agent. Hit next to continue."

Section "Uninstall"
  Call un.ProActiveAgent
SectionEnd

Function un.ProActiveAgent
  !insertmacro Log "Uninstalling ProActiveAgent ..."
  
  # Check user admin rights
  System::Call "kernel32::GetModuleHandle(t 'shell32.dll') i .s"
  System::Call "kernel32::GetProcAddress(i s, i 680) i .r0"
  System::Call "::$0() i .r0"
  DetailPrint "Check: Current user is admin? $0"
  StrCmp $0 '0' 0 +3
  MessageBox MB_OK "Adminstrator rights are required to uninstall the ProActive Agent."
  Abort

  Call un.TerminateAgentForAgent

  # For all users
  SetShellVarContext all
  MessageBox MB_OKCANCEL "This will delete $INSTDIR and all subdirectories and files?" /SD IDOK IDOK DoUninstall
  Abort "Quiting the uninstall process"

  DoUnInstall:
    # Ask the user if he wants to keep the configuration files
    # In silent mode, we remove everything
    MessageBox MB_YESNO "Delete configuration files from $INSTDIR\config ?" /SD IDNO IDNO keepConfigLabel

    SetOutPath $INSTDIR\config
    Delete "PAAgent-config.xml"
    Delete "PAAgent-config-planning-day-only.xml"
    Delete "PAAgent-config-planning-night-we.xml"
    SetOutPath $INSTDIR
    RMDir /r "$INSTDIR\config"

    keepConfigLabel:
      # On x64 we have to explicitely set the registery view
      #${If} ${RunningX64}
      #  SetRegView 64
      #${EndIf}

      # Close the ProActive Agent Control
;      Push ""
;      Push "ProActive Agent Control"
;      Call un.FindWindowClose

      SetOutPath $INSTDIR

      # Check if the service is installed and delete it
      !insertmacro SERVICE "stop" ${SERVICE_NAME} "" "un."
      !insertmacro SERVICE "delete" ${SERVICE_NAME} "" "un."
      #ExecWait "$INSTDIR\AgentFirstSetup.exe -u"

      # Delete regkey from uninstall
      DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent"
      # Delete entry from auto start
      DeleteRegValue HKLM "Software\Microsoft\Windows\CurrentVersion\Run" "ProActiveAgent"
      DeleteRegKey HKLM "Software\ProActiveAgent"

      # Remove the screen saver
      Delete $SYSDIR\ProActiveSSaver.scr

      # Remove all known files except config directory from $INSTDIR
      Delete "$INSTDIR\xml\agent-windows.xsd"
      Delete "$INSTDIR\xml\agent-common.xsd"
      Delete "$INSTDIR\xml\agent-old.xsd"
      RMDir /r "$INSTDIR\xml"
      Delete "$INSTDIR\doc\ProActive Agent Documentation.pdf"
      RMDir /r "$INSTDIR\doc"
      Delete "ConfigParser.dll"
      Delete "ConfigParserOLD.dll"
      Delete "ProActiveAgent.exe"
      Delete "log4net.dll"
      Delete "log4net.config"
      Delete "parunas.exe"
      Delete "pacrypt.dll"
      Delete "InJobProcessCreator.exe"
      Delete "JobManagement.dll"
      Delete "icon.ico"
      Delete "acl.dat"
      Delete "delete_temp.bat"
      Delete "restrict.dat"
      Delete "ListNetworkInterfaces.class"
      Delete "LICENSE.txt"
      Delete "configuration.ini"
      Delete "uninstall.exe"
      Delete "ProActiveAgent-log.txt"
      Delete "AgentForAgent.exe"
      Delete "SubInACL.msi"
      Delete "SetACL.exe"
      RMDir /r "$SMPROGRAMS\ProActiveAgent"
      SetShellVarContext current # reset to current user
FunctionEnd


Function Test
         DetailPrint "sdfsdf"
FunctionEnd

Function un.TerminateAgentForAgent
  # See: http://nsis.sourceforge.net/Find_and_Close_or_Terminate
  Push $0 ; window handle
  Push $1
  Push $2 ; process handle

  FindWindow $0 "" "${WND_TITLE}"
  IntCmp $0 0 finished
  goto ask_user

  ask_user:
    MessageBox MB_YESNO "An instance of the program is running. Do you want to terminate it?" /SD IDYES IDYES terminate IDNO abort

  terminate:
    System::Call 'user32.dll::GetWindowThreadProcessId(i r0, *i .r1) i .r2'
    System::Call 'kernel32.dll::OpenProcess(i ${SYNC_TERM}, i 0, i r1) i .r2'
    SendMessage $0 ${WM_CLOSE} 0 0 /TIMEOUT=${TO_MS}
    System::Call 'kernel32.dll::WaitForSingleObject(i r2, i ${TO_MS}) i .r1'
    IntCmp $1 0 +2
    System::Call 'kernel32.dll::TerminateProcess(i r2, i 0) i .r1'
    System::Call 'kernel32.dll::CloseHandle(i r2) i .r1'
    FindWindow $0 "" "${WND_TITLE}"
    IntCmp $0 0 finished
    Sleep 2000
    Goto ask_user

  abort:
    Abort

  finished:
    Pop $2
    Pop $1
    Pop $0

FunctionEnd

; My time functions

; GetCurrentTime
; Written by Saivert
; Description:
;   Uses System.dll plugin to call Win32's GetTimeFormat in order
;   to get the current time as a formatted string.
Function GetCurrentTime
  Push $R9
  Push $R8

  System::Alloc ${NSIS_MAX_STRLEN}
  Pop $R8
  System::Call 'kernel32::GetTimeFormat(i0,i0,i0,i0,iR8,i${NSIS_MAX_STRLEN}) i..'
  System::Call '*$R8(&t${NSIS_MAX_STRLEN} .R9)'
  System::Free $R8

  Pop $R8
  Exch $R9
FunctionEnd

; GetCurrentDate
; Written by Saivert
; Description:
;   Uses System.dll plugin to call Win32's GetDateFormat in order
;   to get the current date as a formatted string.
Function GetCurrentDate
  Push $R9
  Push $R8

  System::Alloc ${NSIS_MAX_STRLEN}
  Pop $R8
  System::Call 'kernel32::GetDateFormat(i0,i0,i0,i0,iR8,i${NSIS_MAX_STRLEN}) i..'
  System::Call '*$R8(&t${NSIS_MAX_STRLEN} .R9)'
  System::Free $R8

  Pop $R8
  Exch $R9
FunctionEnd