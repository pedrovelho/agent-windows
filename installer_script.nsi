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
!include DotNetVer.nsh
!include servicelib.nsh
!include UserManagement.nsh

#################################################################
# Product version, service name and service description that
# appears in the services.msc panel
#################################################################

; Uncomment one of these lines to build the standalone version
;!define STANDALONE_X86 "x86"
;!define STANDALONE_X64 "x64"

!ifdef STANDALONE_X64
     !define ARCH ${STANDALONE_X64}
     !define SUFIX "${ARCH} Standalone with JRE and Scheduling Worker"
     !define FILENAME_SUFIX "-${ARCH}-standalone"
     !define STANDALONE ""
!endif

!ifdef STANDALONE_X86
     !define ARCH ${STANDALONE_X86}
     !define SUFIX "${ARCH} Standalone with JRE and Scheduling Worker"
     !define FILENAME_SUFIX "-${ARCH}-standalone"
     !define STANDALONE ""
!endif

!ifndef STANDALONE
     !define SUFIX ""
     !define FILENAME_SUFIX ""
!endif

!define VERSION "2.4.3"
!define SERVICE_NAME "ProActiveAgent"
!define SERVICE_DESC "The ProActive Agent enables desktop computers as an important source of computational power"

VIProductVersion                 "${VERSION}.0"
VIAddVersionKey ProductName      "ProActive Agent"
VIAddVersionKey Comments         "www.activeeon.com"
VIAddVersionKey CompanyName      "Activeeon"
VIAddVersionKey LegalCopyright   "Activeeon"
VIAddVersionKey FileDescription  "Installer of the ProActive Agent ${VERSION} ${SUFIX}"
VIAddVersionKey FileVersion      ${VERSION}
VIAddVersionKey ProductVersion   ${VERSION}
VIAddVersionKey InternalName     "ProActiveAgent"
VIAddVersionKey LegalTrademarks  "Copyright (C) Activeeon 2014"
VIAddVersionKey OriginalFilename "ProActiveAgent-${VERSION}${FILENAME_SUFIX}-setup.exe"


#################################################################
# Default config filename and absolute filepath
#################################################################
!define CONFIG_NAME "PAAgent-config.xml"
!define DEFAULT_CONFIG_PATH "$INSTDIR\config\${CONFIG_NAME}"
!define CONFIG_DAY_NAME "PAAgent-config-planning-day-only.xml"
!define CONFIG_NIGHT_NAME "PAAgent-config-planning-night-we.xml"

#################################################################
# Installer log filename and absolute filepath
#################################################################
!define INSTALL_LOG_NAME "install.log"
!define INSTALL_LOG_PATH "$INSTDIR\${INSTALL_LOG_NAME}"

#################################################################
# SetACL tool log filename and absolute filepath
#################################################################
!define SETACL_LOG_NAME "setacl.log"
!define SETACL_LOG_PATH "$INSTDIR\${SETACL_LOG_NAME}"

#################################################################
# SIDs definitions
#################################################################
!define ALL_USERS_SID "S-1-1-0"
!define LOCAL_SYSTEM_SID "S-1-5-18"
!define ADMINISTRATORS_SID "S-1-5-32-544"
!define PERFORMANCE_MONITOR_SID "S-1-5-32-558"

#################################################################
# Privileges required by the ProActive Runtime Account
#################################################################
!define SERVICE_LOGON_RIGHT "SeServiceLogonRight"
!define SE_INCREASE_QUOTA_NAME "SeIncreaseQuotaPrivilege"
!define SE_ASSIGNPRIMARYTOKEN_NAME "SeAssignPrimaryTokenPrivilege"

!define PAGE_FILE "serviceInstallPage.ini"
!define TXT_CONF "Field 9"
!define TXT_LOGSDIR "Field 10"
!define CHK_LOGSHOME "Field 12"
!define CHK_ALLOWANY "Field 11"
!define TXT_DOMAIN "Field 15"
!define TXT_USERNAME "Field 4"
!define TXT_PASSWORD "Field 5"

# Fixed parameters for TerminateAgentForAgent function
!define WND_TITLE "ProActive Agent Control"
!define TO_MS 2000
!define SYNC_TERM 0x0010000

#################################################################
# Default Account Username and Password
#################################################################
!define DEFAULT_USERNAME "proactive"
!define DEFAULT_PASSWORD "Community1."

#################################################################
# Variable filled in ReadSetupArguments and used in
# InstallProActiveAgent.
#################################################################
var AccountDomain
var AccountUsername
var AccountPassword
var AllowEveryone
var ConfigDir
var LogsDir
var UseAccountHome
Var Hostname

CRCCheck on

Name "ProActive Agent ${VERSION} ${SUFIX}"
OutFile ProActiveAgent-${VERSION}${FILENAME_SUFIX}-setup.exe

LicenseText "This program is Licensed under the GNU General Public License (GPL)."
LicenseData "LICENSE.txt"

#####################################################################
# By default the installation directory is "Program Files" however
# on x64 architecture it will be translated as "Program Files (x86)"
#####################################################################
InstallDir $PROGRAMFILES\ProActiveAgent

ComponentText "This will install ProActive Agent on your computer. Select which optional components you want installed."

DirText "Choose a directory to install in to:"

AutoCloseWindow true

Page License
Page Components
Page Directory
Page Custom ConfigureSetupPage HandleSetupArguments
Page Instfiles

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
!ifndef LOGON32_LOGON_BATCH
!define LOGON32_LOGON_BATCH 4
!endif
!ifndef LOGON32_LOGON_INTERACTIVE
!define LOGON32_LOGON_INTERACTIVE 2
!endif
!ifndef LOGON32_LOGON_NETWORK
!define LOGON32_LOGON_NETWORK 3
!endif
!ifndef LOGON32_PROVIDER_DEFAULT
!define LOGON32_PROVIDER_DEFAULT 0
!endif

!ifmacrondef DoLogonUser
  ; Logs on a user, and returns their login token in $R0
  ; R0: token
  ; R8: call return
  !macro DoLogonUser Domain Username Password
     System::Call "${LogonUser}('${Username}', '${Domain}', '${Password}', ${LOGON32_LOGON_NETWORK}, ${LOGON32_PROVIDER_DEFAULT}, .R0) .R8"
  !macroend
!endif

!ifmacrondef DoLogonUserBatch
  ; Logs on a user, and returns their login token in $R0
  ; R0: token
  ; R8: call return
  !macro DoLogonUserBatch Domain Username Password
     System::Call "${LogonUser}('${Username}', '${Domain}', '${Password}', ${LOGON32_LOGON_BATCH}, ${LOGON32_PROVIDER_DEFAULT}, .R0) .R8"
  !macroend
!endif

!ifmacrondef DoLogonUserInteractive
  ; Logs on a user, and returns their login token in $R0
  ; R0: token
  ; R8: call return
  !macro DoLogonUserInteractive Domain Username Password
     System::Call "${LogonUser}('${Username}', '${Domain}', '${Password}', ${LOGON32_LOGON_INTERACTIVE}, ${LOGON32_PROVIDER_DEFAULT}, .R0) .R8"
  !macroend
!endif

!ifmacrondef DoLogoffUser
  ; Logs off a user token (Returned from DoLogonUser)
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
# Removes character or string from another string
#####################################################################
!macro StrStrip Str InStr OutVar
 Push '${InStr}'
 Push '${Str}'
  Call StrStrip
 Pop '${OutVar}'
!macroend

; Usage:
; ${StrStrip}  "ja" "jaja la" $R0
; $R0 == " la"
!define StrStrip '!insertmacro StrStrip'

#################################################################
# This is a simple way to replace the occurrences of a certain
# string throughout the lines of a file. This I use to turn
# templates for configuration files into 'real' ones, but
# for sure you may replace everything.
#################################################################
!macro _ReplaceInFile SOURCE_FILE SEARCH_TEXT REPLACEMENT
  Push "${SOURCE_FILE}"
  Push "${SEARCH_TEXT}"
  Push "${REPLACEMENT}"
  Call RIF
!macroend

#####################################################################
# Logs into the detailed gui section and into the log file
#####################################################################
!macro Log str
  ; Log to NSIS console
  DetailPrint "${str}"
  IfFileExists $INSTDIR 0 +2 ; check exists
  nsislog::log ${INSTALL_LOG_PATH} "${str}"
  ; Log to stdout
  System::Call 'kernel32::GetStdHandle(i -11)i.r9'
  System::Call 'kernel32::AttachConsole(i -1)'
  FileWrite $9 "${str}$\r$\n"
!macroend

#################################################################
# Simulates a keyboard key press to return to prompt after install
#################################################################
!define VK_RETURN             0x0D
!define keybd_event "!insertmacro macro_keybd_event"
!macro macro_keybd_event setkey intkey
  !ifndef keybd
  !define keybd
  !define KEYEVENTF_EXTENDEDKEY 0x0001
  !define KEYEVENTF_KEYUP       0x0002
  !endif
  System::Store S
  ${For} $0 1 ${intkey}
    System::Call "user32::keybd_event(i${setkey}, i0x45, i${KEYEVENTF_EXTENDEDKEY}|0, i0)"
    System::Call "user32::keybd_event(i${setkey}, i0x45, i${KEYEVENTF_EXTENDEDKEY}|${KEYEVENTF_KEYUP}, i0)"
  ${Next}
  System::Store L
!macroend

#################################################################
# Installs ProActive Screen Saver
#################################################################
Function InstallProActiveScreenSaver
        SetOutPath $SYSDIR
        File "bin\Release\ProActiveSSaver.scr"
FunctionEnd

#################################################################
# Creates the shortcuts
#################################################################
Function CreateDesktopShortCuts
        SetShellVarContext all ; All users
        CreateDirectory "$SMPROGRAMS\ProActiveAgent"
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Control.lnk" "$INSTDIR\AgentForAgent.exe" "" "$INSTDIR\icon.ico" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\Uninstall ProActive Agent.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
        CreateShortCut  "$SMPROGRAMS\ProActiveAgent\ProActive Agent Documentation.lnk" "$INSTDIR\doc\ProActive Agent Documentation.pdf" "" "$INSTDIR\doc\ProActive Agent Documentation.pdf" 0
        SetShellVarContext current ; reset to current user
FunctionEnd

#####################################################################
# Runs uninstaller if available
#####################################################################
Function RollbackIfSilent
  ${If} ${Silent}
    !insertmacro Log "Running uninstaller ..."
    ${If} ${FileExists} '$INSTDIR\uninstall.exe'
      ExecWait '"$INSTDIR\uninstall.exe" /S ?_=$INSTDIR'
    ${EndIf}
    !insertmacro Log "Uninstall finished ..."
  ${EndIf}
FunctionEnd

##########################################################################################################################################
# On init peforms the following checks:
# - admin rights
# - Microsoft .NET Framework 3.5
# - Previous version of the unistaller
#
# Note that in this function the following statement:
# !insertmacro Log "... message ..."
# will not work since the installdir is unknown yet so we do not log anything and instead we set an error level with the SetErrorLevel
# 3 - user is not admin
# 4 - Unable to find Microsoft .NET Framework 3.5
##########################################################################################################################################
Function .onInit
  !insertmacro Log "$\r"
  !insertmacro Log "$\r---------------------------------------------------"
  !insertmacro Log "$\r"
  ; Print the current date and time into the installation log file
  Call GetCurrentDate
  Pop $R0
  Call GetCurrentTime
  Pop $R1
  !insertmacro Log "\r$R0 - $R1 Installing ProActiveAgent v${VERSION} ..."
  StrCpy $R0 ""
  StrCpy $R1 ""
  
  ; Forbid the execution of 64bit installer on 32 bits
  ${If} ${RunningX64}
    !insertmacro Log "Running on x64 architecture ..."
  ${Else}
    !insertmacro Log "Running on x86 architecture ..."
    !ifdef STANDALONE_X64
       ${IfNot} ${Silent}
          MessageBox MB_OK "Cannot run a x64 installer on a x86 architecture." /SD IDOK
       ${EndIf}
       !insertmacro Log "Cannot run a x64 installer on a x86 architecture."
       SetErrorLevel 3
       Abort
    !endif
  ${EndIf}
  
  ; Check user admin rights
  !insertmacro Log "Checking admin rights ..."
  System::Call "kernel32::GetModuleHandle(t 'shell32.dll') i .s"
  System::Call "kernel32::GetProcAddress(i s, i 680) i .r0"
  System::Call "::$0() i .r0"
  ${If} $R0 == '0'
     ${If} ${Silent}
       !insertmacro Log "!! Administrator rights are required to install the ProActive Agent !!"
     ${Else}
        MessageBox MB_OK "Administrator rights are required to install the ProActive Agent." /SD IDOK
     ${EndIf}
     SetErrorLevel 3
     Abort
  ${EndIf}

  ; Parse args
  ${If} ${Silent}
    !insertmacro Log "Running silent installation ..."
    ${GetParameters} $0 ; $0 contains all args
    ; !insertmacro Log "Specified parameters: $0"
    ; Checking /UN to run the uninstaller
    ${GetOptions} $0 "/UN" $1
    IfErrors noUninstallLABEL
    !insertmacro Log "The /UN is specified, running uninstaller and exiting ..."
    ReadRegStr $0 HKLM "Software\ProActiveAgent" "AgentLocation"
    ${If} ${FileExists} $0
      ExecWait '"$0\uninstall.exe" /S ?_=$0'
    ${EndIf}
    Delete ${INSTALL_LOG_PATH}
    Abort
    noUninstallLABEL:
    ClearErrors
    ; Checking /USE_ACC to use account home for Config Dir and Logs Dir
    ${GetOptions} $0 "/USE_ACC" $UseAccountHome
    IfErrors noUseAccLABEL
    !insertmacro Log "The /USE_ACC is specified, using runtime account home for config and logs ..."
    StrCpy $UseAccountHome "1"
    noUseAccLABEL:
    ClearErrors
    ; Checking /ALLOW to allow everyone to control the service
    ${GetOptions} $0 "/ALLOW" $AllowEveryone
    IfErrors noAllowLABEL
    !insertmacro Log "The /ALLOW is specified, allowing everyone to control the agent ..."
    StrCpy $AllowEveryone "1"
    noAllowLABEL:
    ClearErrors
    ; Parsing locations
    ${GetOptions} $0 "/CONFIG_DIR=" $ConfigDir
    ${GetOptions} $0 "/LOG_DIR=" $LogsDir
    ; Parsing Runtime Account domain, username and password
    ${GetOptions} $0 "/DOMAIN=" $AccountDomain
    ${GetOptions} $0 "/USER=" $AccountUsername
    ${GetOptions} $0 "/PASSWORD=" $AccountPassword
    ClearErrors
  ${EndIf}
  
  ; Read hostname
  !insertmacro GetServerName $Hostname
  ${StrStrip} "\\" $Hostname $Hostname ; Remove leading \\ to avoid AGENT-192 (bugs.activeeon.com)

  ; Check if .NET framework 3.5 is installed
  ${IfNot} ${HasDotNet3.5}
    !insertmacro Log "!! Unable to find Microsoft .NET Framework 3.5 !!"
    ${IfNot} ${Silent}
       MessageBox MB_OK "Unable to find Microsoft .NET Framework 3.5" /SD IDOK
    ${EndIf}
    Abort
  ${EndIf}

  ; Check if a previous unistaller is available, in gui mode ask the user
  ReadRegStr $0 HKLM "Software\ProActiveAgent" "AgentLocation"
  ${If} ${FileExists} '$0\uninstall.exe'
    ${If} ${Silent}
      ; The silent mode always uninstalls the previous version
      ; Loop until the uninstaller is still available and try again
      !insertmacro Log "Uninstalling previous version from $0 ..."
      ;nsExec::Exec '"$0\uninstall.exe" /S ?_=$0'
      ExecWait '"$0\uninstall.exe" /S _?=$0'
      Sleep 5000
      !insertmacro Log "Previous version uninstalled sucessfully ..."
    ${Else}
      MessageBox MB_YESNO "The previous version of the ProActive Windows Agent must be uninstalled. Run the uninstaller ?" /SD IDYES IDYES runUninstallerLABEL
      Abort
      runUninstallerLABEL:
      ExecWait '"$0\uninstall.exe" _?=$0'
    ${EndIf}
  ${EndIf}
FunctionEnd

Function ConfigureSetupPage
  ReserveFile ${PAGE_FILE}
  !insertmacro MUI_INSTALLOPTIONS_EXTRACT ${PAGE_FILE}
  ; Set default location for configuration file
  !insertmacro MUI_INSTALLOPTIONS_WRITE ${PAGE_FILE} "${TXT_CONF}" State "$INSTDIR\config"
  ; Set default location for logs directory
  !insertmacro MUI_INSTALLOPTIONS_WRITE ${PAGE_FILE} "${TXT_LOGSDIR}" State "$INSTDIR\logs"
  ; Set default account domain, username and password
  !insertmacro MUI_INSTALLOPTIONS_WRITE ${PAGE_FILE} "${TXT_DOMAIN}" State $Hostname
  !insertmacro MUI_INSTALLOPTIONS_WRITE ${PAGE_FILE} "${TXT_USERNAME}" State ${DEFAULT_USERNAME}
  !insertmacro MUI_INSTALLOPTIONS_WRITE ${PAGE_FILE} "${TXT_PASSWORD}" State ${DEFAULT_PASSWORD}
  ; Disable "Use service account home"
  ; !insertmacro GROUPCONTROLS "${PAGE_FILE}" "${SEL_INSTLOC}" "${CHK_LOGSHOME}|" "1"
  ; Display the custom page
  !insertmacro MUI_INSTALLOPTIONS_DISPLAY ${PAGE_FILE}
FunctionEnd

Function HandleSetupArguments
  Call ReadSetupArguments
  Call ProcessSetupArguments
FunctionEnd

Function ReadSetupArguments
  ; Handle notify event of checkbox "Use service account home"
  !insertmacro MUI_INSTALLOPTIONS_READ $0 "${PAGE_FILE}" "Settings" State
  ${Switch} "Field $0"
    ${Case} "${CHK_LOGSHOME}"
      !insertmacro GROUPCONTROLS "${PAGE_FILE}" "${CHK_LOGSHOME}" "${TXT_LOGSDIR}|${TXT_CONF}" "1"
      Abort
  ${EndSwitch}
  # Check "Use service account home" for logs location stored in R7
  !insertmacro MUI_INSTALLOPTIONS_READ $UseAccountHome ${PAGE_FILE} "${CHK_LOGSHOME}" State

  !insertmacro MUI_INSTALLOPTIONS_READ $ConfigDir ${PAGE_FILE} "${TXT_CONF}" State
  !insertmacro MUI_INSTALLOPTIONS_READ $LogsDir ${PAGE_FILE} "${TXT_LOGSDIR}" State
  
  ; CHECK ACCOUNT FIELDS
  !insertmacro MUI_INSTALLOPTIONS_READ $AllowEveryone ${PAGE_FILE} "${CHK_ALLOWANY}" State
  !insertmacro MUI_INSTALLOPTIONS_READ $AccountDomain ${PAGE_FILE} "${TXT_DOMAIN}" State
  !insertmacro MUI_INSTALLOPTIONS_READ $AccountUsername ${PAGE_FILE} "${TXT_USERNAME}" State
  !insertmacro MUI_INSTALLOPTIONS_READ $AccountPassword ${PAGE_FILE} "${TXT_PASSWORD}" State
FunctionEnd

Function ProcessSetupArguments
  ; If UseAccountHome is checked then skip locations checks
  ${If} $UseAccountHome == "1"
    Goto skipLocationChecksLABEL
  ${EndIf}

  ; Check config dir, if undefined set to default value
  ${If} $ConfigDir == ""
    !insertmacro Log "Config Dir not specified, using default: $INSTDIR\config ..."
    StrCpy $ConfigDir "$INSTDIR\config"
  ${Else}
    ${If} $ConfigDir == "$INSTDIR\config"
      !insertmacro Log "Using default Config Dir: $INSTDIR\config ..."
    ${Else}
      IfFileExists $ConfigDir configDirExistLABEL 0
      ${If} ${Silent}
        !insertmacro Log "!! The specified Config Dir: $ConfigDir does not exists !!"
        Abort
      ${Else}
        MessageBox MB_YESNO "The specified Config Dir does not exist, would you like to create it ?" /SD IDNO IDYES createConfigDirLABEL
        Abort
        createConfigDirLABEL:
        CreateDirectory $ConfigDir
      ${EndIf}
      configDirExistLABEL:
    ${EndIf}
  ${EndIf}
  
  ; Check logs dir, if undefined set to default value
  ${If} $LogsDir == ""
    !insertmacro Log "Logs Dir not specified, using default: $INSTDIR\logs ..."
    StrCpy $LogsDir "$INSTDIR\logs"
  ${Else}
    ${If} $LogsDir == "$INSTDIR\logs"
      !insertmacro Log "Using default Logs Dir: $INSTDIR\logs ..."
    ${Else}
      IfFileExists $LogsDir logsDirectoryExistsLABEL 0
      ${If} ${Silent}
        !insertmacro Log "!! The specified Logs Dir does not exists Invalid logs directory !!"
        Call RollbackIfSilent
      ${Else}
        MessageBox MB_YESNO "The specified Logs Dir does not exist, would you like to create it ?" /SD IDNO IDYES createLogsDirLABEL
        Abort
        createLogsDirLABEL:
        CreateDirectory $LogsDir
      ${EndIf}
      Abort
      logsDirectoryExistsLABEL:
    ${EndIf}
  ${EndIf}

  skipLocationChecksLABEL:

  ; Check Account Domain
  ${If} $AccountDomain == ""
    ${If} ${Silent}
      !insertmacro Log "Account Domain not specified, using default: $Hostname ..."
      StrCpy $AccountDomain $Hostname
    ${Else}
      MessageBox MB_OK "Please enter a valid Domain" /SD IDOK
      Abort
    ${EndIf}
  ${EndIf}
  
  ; Check Account Username
  ${If} $AccountUsername == ""
    ${If} ${Silent}
      !insertmacro Log "Account Username not specified, using default: ${DEFAULT_USERNAME} ..."
      StrCpy $AccountUsername ${DEFAULT_USERNAME}
    ${Else}
      MessageBox MB_OK "Please enter a valid Username" /SD IDOK
      Abort
    ${EndIf}
  ${EndIf}

  ; Check Account Password
  ${If} $AccountPassword == ""
    ${If} ${Silent}
      !insertmacro Log "Account Password not specified, using default ..."
      StrCpy $AccountPassword ${DEFAULT_PASSWORD}
    ${Else}
      MessageBox MB_OK "Please enter a valid Password" /SD IDOK
      Abort
    ${EndIf}
  ${EndIf}

  checkAccountLABEL:
  !insertmacro Log "Trying to logon as $AccountUsername on domain: $AccountDomain ..."
  !insertmacro DoLogonUser $AccountDomain $AccountUsername $AccountPassword
  
  ${If} $R8 == 0
     ; DoLogonUser failed
     !insertmacro Log "Unable to logon using DoLogonUser return value: $R8 token: $R0"
     
     ; Try with DoLogonUserBatch
     !insertmacro DoLogonUserBatch $AccountDomain $AccountUsername $AccountPassword
     
     ${If} $R8 == 0
        ; DoLogonUserBatch failed
        !insertmacro Log "Unable to logon using DoLogonUserBatch return value: $R8 token: $R0"
        
        ; Try with DoLogonUserInteractive
        !insertmacro DoLogonUserInteractive $AccountDomain $AccountUsername $AccountPassword
        
        ${If} $R8 == 0
           ; DoLogonUserInteractive failed
           !insertmacro Log "Unable to logon using DoLogonUserInteractive return value: $R8 token: $R0"
           ; This is bad and there is nothing to do
        ${EndIf}
     ${EndIf}
  ${EndIf}
  
  StrCmp $R8 0 unableToLogLABEL loggedLABEL
  
  # If unable to logon using default account maybe the account does not exists or password is incorrect
  unableToLogLABEL:
  !insertmacro Log "Unable to logon as $AccountUsername on domain: $AccountDomain Trying to create a user on the local machine ..."
  # Check if cannot logon but the user exists
  UserMgr::GetUserInfo $AccountUsername "EXISTS"
  Pop $0
  ${If} $0 == "OK"
     # If exists set never expiring password
     UserMgr::SetUserInfo $AccountUsername "PASSWORD" $AccountPassword
     Pop $0
     !insertmacro Log "SetUserInfo(password) returns: $0"
     ${If} $0 == "ERROR 2245" # Password requirements are not met
        !insertmacro Log "!! The password requirements are not met !!"
        ${IfNot} ${Silent}
          MessageBox MB_OK "The password does not meet the password policy requirements. Check the minimum password length, password complexity and password history requirements." /SD IDOK
        ${EndIf}
        Abort
     ${EndIf}
     UserMgr::SetUserInfo $AccountUsername "PASSWD_NEVER_EXPIRES" "YES"
     Pop $0
     !insertmacro Log "SetUserInfo(expire): $0"
     
     ; We now should be able to logon
     !insertmacro Log "Trying to logon as $AccountUsername on domain: $AccountDomain ..."
     !insertmacro DoLogonUser $AccountDomain $AccountUsername $AccountPassword
     ${If} $R8 == 0 ; Abort if unable to Logon
       !insertmacro Log "Unable to logon using DoLogonUser return value: $R8 token: $R0"
       !insertmacro Log "Please create an account manually ..."
       ${IfNot} ${Silent}
          MessageBox MB_OK "The account $AccountUsername exists but the logon on $AccountDomain failed, probably the logon as this account is forbidden, please try to specify the FQDN as Domain or specify another valid account." /SD IDOK
       ${EndIf}
       Abort
     ${EndIf}
  ${Else}
     ${If} $0 == "ERROR 2221" # 2221 means the user name could not be found

        ; In gui install check if default account, if not ask if the user wants to create a new account
        ${IfNot} ${Silent}
          ${If} $AccountUsername == ${DEFAULT_USERNAME}
          ${AndIf} $AccountPassword == ${DEFAULT_PASSWORD}
             !insertmacro Log "Creating new default account ..."
          ${Else}
             MessageBox MB_YESNO "The local account $AccountUsername does not exist, would you like to create it ?" /SD IDNO IDYES +2
             Abort
          ${EndIf}
        ${EndIf}

        # Create the account with never expiring password, runasme required privileges and build this account env
        UserMgr::CreateAccount $AccountUsername $AccountPassword "The ProActive Agent runs a Java Virtual Machine under this account."
        Pop $0
        !insertmacro Log "UserMgr::CreateAccount returns: $0"
        ${If} $0 == "ERROR 2245" # Password requirements are not met
          !insertmacro Log "!! The password requirements are not met !!"
          ${IfNot} ${Silent}
            MessageBox MB_OK "The password does not meet the password policy requirements. Check the minimum password length, password complexity and password history requirements." /SD IDOK
          ${EndIf}
          Abort
        ${EndIf}
        UserMgr::AddPrivilege $AccountUsername ${SE_INCREASE_QUOTA_NAME}
        Pop $0
        !insertmacro Log "UserMgr::AddPrivilege(quota) returns: $0"
        UserMgr::AddPrivilege $AccountUsername ${SE_ASSIGNPRIMARYTOKEN_NAME}
        Pop $0
        !insertmacro Log "UserMgr::AddPrivilege(assign) returns: $0"
        UserMgr::BuiltAccountEnv $AccountUsername $AccountPassword
        Pop $0
        !insertmacro Log "UserMgr::BuiltAccountEnv returns: $0"
        UserMgr::SetUserInfo $AccountUsername "PASSWD_NEVER_EXPIRES" "YES"
        Pop $0
        !insertmacro Log "SetUserInfo(expire): $0"
        !insertmacro Log "Trying to logon after account creation as $AccountUsername on domain: $AccountDomain ..."
        !insertmacro DoLogonUser $AccountDomain $AccountUsername $AccountPassword
        ${If} $R8 == 0 ; Abort if unable to Logon
          !insertmacro Log "Unable to logon using DoLogonUser return value: $R8 token: $R0"
          !insertmacro Log "Please create an account manually ..."
          ${IfNot} ${Silent}
             MessageBox MB_OK "Unable to logon as $AccountUsername on $AccountDomain after account creation please specify a valid local or domain account." /SD IDOK
          ${EndIf}
          Abort
        ${EndIf}
     ${Else}
        !insertmacro Log "Unable to logon as $AccountUsername and to know if username exists. GetUserInfo returns: $0"
        Abort
     ${EndIf}
  ${EndIf}
  
  loggedLABEL:
  # The user is logged it means the password is correct
  !insertmacro Log "Logging off ..."
  # Logoff user (preserve $R5)
  StrCpy $0 $R5
  !insertmacro DoLogoffUser $R0
  StrCpy $R5 $0
  
  ; Check privileges required for RunAsMe mode
  UserMgr::HasPrivilege $AccountUsername ${SE_INCREASE_QUOTA_NAME}
  Pop $0
  ${If} $0 != "TRUE"
    !insertmacro Log "Does $AccountUsername have the privilege SE_INCREASE_QUOTA_NAME ? UserMgr::HasPrivilege returns $0 ..."
    !insertmacro Log "In order to use RunAsMe mode, the account $AccountUsername must have 'Adjust memory quotas for a process' and 'Replace a process-level token' privileges. In the 'Administrative Tools' of the 'Control Panel' open the 'Local Security Policy'. In 'Security Settings', select 'Local Policies' then select 'User Rights Assignments'. Finally, in the list of policies open the corresponding properties and add the account $AccountUsername."
  ${EndIf}
  UserMgr::HasPrivilege $AccountUsername ${SE_ASSIGNPRIMARYTOKEN_NAME}
  Pop $0
  ${If} $0 != "TRUE"
    !insertmacro Log "Does $AccountUsername have the privilege SE_ASSIGNPRIMARYTOKEN_NAME ? UserMgr::HasPrivilege returns $0 ..."
    !insertmacro Log "In order to use RunAsMe mode, the account $AccountUsername must have 'Adjust memory quotas for a process' and 'Replace a process-level token' privileges. In the 'Administrative Tools' of the 'Control Panel' open the 'Local Security Policy'. In 'Security Settings', select 'Local Policies' then select 'User Rights Assignments'. Finally, in the list of policies open the corresponding properties and add the account $AccountUsername."
  ${EndIf}
      
  ; Check if the account is part of 'Performace Monitor Group'
  ; The function UserMgr::IsMemberOfGroup does not work.
  ; Instead we use the UserMgr::GetUserNameFromSID to check if the group exists if so
  ; the user is added using a macro.
  !insertmacro Log "Checking runtime account member of Performace Monitor Group ..."
  UserMgr::GetUserNameFromSID ${PERFORMANCE_MONITOR_SID}
  Pop $0
  ${If} $0 == "ERROR"
    ; The group does not exist, this occur on Windows XP so there is nothing to do
    ${IfNot} ${IsWinXP}
      !insertmacro Log "!! ${PERFORMANCE_MONITOR_SID} does not exist !!"
    ${EndIf}
  ${Else}
    !insertmacro AddUserToGroup1 $Hostname $AccountUsername "558" ; UserMgr::AddToGroup doesn't work
  ${EndIf}
FunctionEnd

#############################
# !! SECTIONS DEFINITIONS !!
#############################
Section "ProActive Agent"
        ; In silent mode, we needs to explicitly handle parameters and installation
        ${If} ${Silent}
           Call ProcessSetupArguments
        ${EndIf}

        !insertmacro Log "Installing into $INSTDIR ..."
        ; Set current dir to installation directory
        SetOutPath $INSTDIR

        ; Write uninstaller utility to able to rollback
        WriteUninstaller uninstall.exe

        !insertmacro Log "Writing registry keys ..."
        ; Write the uninstall keys for Windows
        WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent" "DisplayName" "ProActive Agent (remove only)"
        WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent" "UninstallString" '"$INSTDIR\Uninstall.exe"'

        ; Write into registry the agent home and config location and logs dir
        WriteRegStr HKLM "Software\ProActiveAgent" "AgentLocation" "$INSTDIR"
        ; Write auth data into a restricted access key
        WriteRegStr HKLM "Software\ProActiveAgent\Creds" "domain" $AccountDomain
        WriteRegStr HKLM "Software\ProActiveAgent\Creds" "username" $AccountUsername

        ; Encrypt the password, see AGENT-154
        File "bin\Release\pacrypt.dll" ; the .dll contains C-Signature: int encryptData(wchar_t *input, wchar_t *output)
        !insertmacro Log "Encrypting password ..."
        StrCpy $0 $AccountPassword ; copy register to stack
        System::Call "pacrypt::encryptData(w, w) i(r0., .r1).r2"
        ${If} $2 != 0
           !insertmacro Log "!! Unable to encrypt the password (too long ?). Error $2 !!"
           MessageBox MB_OK "Unable to encrypt the password (too long ?). Error $2" /SD IDOK
           Call RollbackIfSilent
           Abort
        ${EndIf}
        ; Uncomment the following code to chek if the encryption mechanism works correctly
        ; the last message box should show
        ;MessageBox MB_OK "---> $0 , $1 , $2"
        ;System::Call "pacrypt::decryptData(w, w) i(r1., .r4).r0"
        ;MessageBox MB_OK "---> $0 , $1 , $4"
        ; Write encrypted password in registry
        WriteRegStr HKLM "Software\ProActiveAgent\Creds" "password" $1

        ; The command based on SID grants full permissions only for LocalSystem and Administrators keyfile and the registry key
        File "utils\SetACL.exe" ; copy the tool for access restriction
        !insertmacro Log "Restricting keyfile access ..."
        nsExec::Exec '"$INSTDIR\SetACL.exe" -log "${SETACL_LOG_PATH}" -on "$INSTDIR\restrict.dat" -ot file -actn setprot -op "dacl:p_nc;sacl:p_nc" -actn ace -ace "n:${LOCAL_SYSTEM_SID};p:full;s:y" -ace "n:${ADMINISTRATORS_SID};p:full;s:y"'
        !insertmacro Log "Restricting regkey access ..."
        nsExec::Exec '"$INSTDIR\SetACL.exe" -log "${SETACL_LOG_PATH}" -on HKEY_LOCAL_MACHINE\Software\ProActiveAgent\Creds -ot reg -actn setprot -op "dacl:p_nc;sacl:p_nc" -actn ace -ace "n:${LOCAL_SYSTEM_SID};p:full;s:y" -ace "n:${ADMINISTRATORS_SID};p:full;s:y"'

        ; Install the service under the Local System and check if the service was correctly installed
        File "bin\Release\ProActiveAgent.exe" ; copy the service executable
        !insertmacro Log "Installing ProActiveAgent.exe as service ..."
        nsExec::Exec 'cmd.exe /C sc create ${SERVICE_NAME} binPath= "$INSTDIR\ProActiveAgent.exe" DisplayName= "${SERVICE_NAME}" start= auto type= interact type= own & sc description ${SERVICE_NAME} "${SERVICE_DESC}"'
        !insertmacro Log "Checking service installation ..."
        !insertmacro SERVICE "status" ${SERVICE_NAME} '' ""
        Pop $0
        ${If} $0 != "stopped"
          !insertmacro Log "!! Unable to install as service !!"
          MessageBox MB_OK "Unable to install as service. To install manually use sc.exe command" /SD IDOK
          Call RollbackIfSilent
          Abort
        ${EndIf}

        ; Checking the usage of the account home for config and logs
        ${If} $UseAccountHome == "1"
          !insertmacro Log "Using runtime account $AccountUsername for config and logs dir ..."
          ; The goal is to read the path of AppData folder
          UserMgr::GetSIDFromUserName "." $AccountUsername ; Get the SID of the user
          Pop $0
          ${If} ${IsWinXP}
            ; On xp use the standard way, if it is loaded in HKU by SID check in the Volatile Environment
            ReadRegStr $1 HKU "$0\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" AppData
            ; If the result is empty, load the account into the HKU by username the read the same key
            ${If} $1 == ""
              UserMgr::RegLoadUserHive $AccountUsername
              ReadRegStr $1 HKU "$AccountUsername\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders" AppData
              UserMgr::RegUnLoadUserHive $AccountUsername
            ${EndIf}
          ${Else}
            ; On other OS like Vista or 7 use HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList\SID ProfileImagePath
            ; If it is loaded in HKU by SID check in the Volatile Environment
            ReadRegStr $1 HKLM "SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProfileList\$0" ProfileImagePath
            StrCpy $1 "$1\AppData\Roaming"
          ${EndIf}
          CreateDirectory "$1\ProActiveAgent\config"
          StrCpy $ConfigDir "$1\ProActiveAgent\config"
          CreateDirectory "$1\ProActiveAgent\logs"
          StrCpy $LogsDir "$1\ProActiveAgent\logs"
        ${EndIf}

        ; Checking default location for config and log
        !insertmacro Log "Checking use default location ..."
        ${If} $ConfigDir == "$INSTDIR\config"
          IfFileExists $ConfigDir\${CONFIG_NAME} 0 defaultFileNotExistLabel
          MessageBox MB_YESNO "Use existing configuration file $INSTDIR\config\${CONFIG_NAME} ?" /SD IDYES IDNO defaultFileNotExistLabel
          !insertmacro Log "Using existing configuration file ..."
          Goto useExistingConfigLabel
        ${Else} ; If NOT default location
          IfFileExists $ConfigDir dirExistLABEL dirNotExistLABEL ; check exists
          dirNotExistLABEL:
          MessageBox MB_OK "The specified directory $ConfigDir for configuration file doesn't exist" /SD IDOK
          Abort
          dirExistLABEL:
          ; Check if there is already a config file
          IfFileExists "$ConfigDir\${CONFIG_NAME}" askUseLABEL defaultFileNotExistLabel
          askUseLABEL:
          ; Ask the user if he wants to use the existing file (if not the default one will be copied to this dir)
          MessageBox MB_YESNO "Use existing configuration file $ConfigDir\${CONFIG_NAME} ?" /SD IDYES IDNO defaultFileNotExistLabel
          !insertmacro Log "Using existing configuration file ..."
          Goto useExistingConfigLabel
        ${EndIf}

        defaultFileNotExistLabel:
        !insertmacro Log "Writing config files into $ConfigDir ..."
        ; Copy config files then locate java and insert into it
        SetOutPath $ConfigDir
        File "utils\${CONFIG_NAME}"
        File "utils\${CONFIG_DAY_NAME}"
        File "utils\${CONFIG_NIGHT_NAME}"
        !ifdef STANDALONE
                ; In standlaone mode inject schedworker absolute path into <proactiveHome> and <credential> elements values
                !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_NAME}" "<proactiveHome />" "<proactiveHome>$INSTDIR\schedworker</proactiveHome>"
                Delete "$ConfigDir\${CONFIG_NAME}.old"
                !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_NAME}" "<credential />" "<credential>$INSTDIR\schedworker\config\authentication\rm.cred</credential>"
                Delete "$ConfigDir\${CONFIG_NAME}.old"
                !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_DAY_NAME}" "<proactiveHome />" "<proactiveHome>$INSTDIR\schedworker</proactiveHome>"
                Delete "$ConfigDir\${CONFIG_DAY_NAME}.old"
                !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_DAY_NAME}" "<credential />" "<credential>$INSTDIR\schedworker\config\authentication\rm.cred</credential>"
                Delete "$ConfigDir\${CONFIG_DAY_NAME}.old"
                !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_NIGHT_NAME}" "<proactiveHome />" "<proactiveHome>$INSTDIR\schedworker</proactiveHome>"
                Delete "$ConfigDir\${CONFIG_NIGHT_NAME}.old"
                !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_NIGHT_NAME}" "<credential />" "<credential>$INSTDIR\schedworker\config\authentication\rm.cred</credential>"
                Delete "$ConfigDir\${CONFIG_NIGHT_NAME}.old"

                ; In standalone mode a jre bundle will be installed
                StrCpy $0 "$INSTDIR\jre"
                !insertmacro Log "Java home is set to the bundled jre at $0 ..."
        !else
                ; Auto detect java home
                Call LocateJava
                Pop $0
                !insertmacro Log "Java home is located at $0 ..."
        !endif

        !insertmacro Log "Inserting java home into config files ..."
        !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_NAME}" "<javaHome />" "<javaHome>$0</javaHome>"
        !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_DAY_NAME}" "<javaHome />" "<javaHome>$0</javaHome>"
        !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_NIGHT_NAME}" "<javaHome />" "<javaHome>$0</javaHome>"
        ; Sometime the replacement mecanism doesn't remove .old files
        Delete "$ConfigDir\${CONFIG_NAME}.old"
        Delete "$ConfigDir\${CONFIG_DAY_NAME}.old"
        Delete "$ConfigDir\${CONFIG_NIGHT_NAME}.old"

        !insertmacro Log "Changing localhost to $Hostname in rm url in the default config files ..."
        !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_NAME}" "localhost" "$Hostname"
        !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_DAY_NAME}" "localhost" "$Hostname"
        !insertmacro _ReplaceInFile "$ConfigDir\${CONFIG_NIGHT_NAME}" "localhost" "$Hostname"
        ; Sometime the replacement mecanism doesn't remove .old files
        Delete "$ConfigDir\${CONFIG_NAME}.old"
        Delete "$ConfigDir\${CONFIG_DAY_NAME}.old"
        Delete "$ConfigDir\${CONFIG_NIGHT_NAME}.old"

        useExistingConfigLabel:
        WriteRegStr HKLM "Software\ProActiveAgent" "ConfigLocation" "$ConfigDir\${CONFIG_NAME}"
        WriteRegStr HKLM "Software\ProActiveAgent" "LogsDirectory" $LogsDir

        ; Write other files
        SetOutPath $INSTDIR
        File "LICENSE.txt"
        File "bin\Release\AgentForAgent.exe"
        File "bin\Release\ConfigParser.dll"
        File "bin\Release\parunas.exe"
        File "utils\icon.ico"
        File "utils\ListNetworkInterfaces.class"
        File "utils\delete_temp.bat"
        File "ProActiveAgent\log4net.config"
        File "ProActiveAgent\lib\log4net.dll"
        File "ProActiveAgent\lib\InJobProcessCreator.exe"
        File "ProActiveAgent\lib\x86\JobManagement.dll"
        SetOutPath $INSTDIR\xml
        File "utils\xml\agent-windows.xsd"
        File "utils\xml\agent-common.xsd"
        SetOutPath $INSTDIR\doc
        File "ProActive Agent Documentation.pdf"

        ;-----------------------------------------------------------------------------------
        ; The standalone contains the schedworker version
        ;-----------------------------------------------------------------------------------
        !ifdef STANDALONE
                SetOutPath $INSTDIR\schedworker
                File "utils\schedworker\LICENSE.txt"
                File "utils\schedworker\LICENSE_EXCEPTION.txt"
                File "utils\schedworker\README.txt"
                File "utils\schedworker\build_id"
                SetOutPath $INSTDIR\schedworker\bin\windows
                File "utils\schedworker\bin\windows\init.bat"
                File "utils\schedworker\bin\windows\rm-start-node.bat"
                SetOutPath $INSTDIR\schedworker\config
                File "utils\schedworker\config\security.java.policy-client"
                SetOutPath $INSTDIR\schedworker\config\authentication
                File "utils\schedworker\config\authentication\rm.cred"
                SetOutPath $INSTDIR\schedworker\config\log4j
                File "utils\schedworker\config\log4j\log4j-defaultNode"
                SetOutPath $INSTDIR\schedworker\config\proactive
                File "utils\schedworker\config\proactive\ProActiveConfiguration.xml"
                SetOutPath $INSTDIR\schedworker\dist\lib
                File "utils\schedworker\dist\lib\ProActive.jar"
                File "utils\schedworker\dist\lib\ProActive_ResourceManager-client.jar"
                File "utils\schedworker\dist\lib\ProActive_ResourceManager-tests.jar"
                File "utils\schedworker\dist\lib\ProActive_ResourceManager.jar"
                File "utils\schedworker\dist\lib\ProActive_SRM-common-client.jar"
                File "utils\schedworker\dist\lib\ProActive_SRM-common.jar"
                File "utils\schedworker\dist\lib\ProActive_Scheduler-client.jar"
                File "utils\schedworker\dist\lib\ProActive_Scheduler-core.jar"
                File "utils\schedworker\dist\lib\ProActive_Scheduler-fsm.jar"
                File "utils\schedworker\dist\lib\ProActive_Scheduler-mapreduce.jar"
                File "utils\schedworker\dist\lib\ProActive_Scheduler-tests.jar"
                File "utils\schedworker\dist\lib\ProActive_Scheduler-worker.jar"
                File "utils\schedworker\dist\lib\ProActive_annotations_CTree.jar"
                File "utils\schedworker\dist\lib\ProActive_examples.jar"
                File "utils\schedworker\dist\lib\ProActive_tests.jar"
                File "utils\schedworker\dist\lib\ProActive_utils.jar"
                File "utils\schedworker\dist\lib\WinHPCWebserviceStubs.jar"
                File "utils\schedworker\dist\lib\antlr-2.7.7.jar"
                File "utils\schedworker\dist\lib\asm-3.2.jar"
                File "utils\schedworker\dist\lib\ast-core-2.4-SNAPSHOT.jar"
                File "utils\schedworker\dist\lib\axiom-api-1.2.7.jar"
                File "utils\schedworker\dist\lib\axiom-dom-1.2.7.jar"
                File "utils\schedworker\dist\lib\axis2-codegen-1.4.jar"
                File "utils\schedworker\dist\lib\axis2-kernel-1.4.1.jar"
                File "utils\schedworker\dist\lib\batik-awt-util.jar"
                File "utils\schedworker\dist\lib\batik-dom.jar"
                File "utils\schedworker\dist\lib\batik-svggen.jar"
                File "utils\schedworker\dist\lib\batik-util.jar"
                File "utils\schedworker\dist\lib\batik-xml.jar"
                File "utils\schedworker\dist\lib\bcel-5.2.jar"
                File "utils\schedworker\dist\lib\bouncycastle.jar"
                File "utils\schedworker\dist\lib\bundlerepository-1.4.3.jar"
                File "utils\schedworker\dist\lib\c3p0-0.9.2-pre4.jar"
                File "utils\schedworker\dist\lib\c3p0-oracle-thin-extras-0.9.2-pre4.jar"
                File "utils\schedworker\dist\lib\cimom.jar"
                File "utils\schedworker\dist\lib\cimrepository.jar"
                File "utils\schedworker\dist\lib\cockpit-0.9.0.jar"
                File "utils\schedworker\dist\lib\cog-jglobus-1.2.jar"
                File "utils\schedworker\dist\lib\cog-ogce.jar"
                File "utils\schedworker\dist\lib\colobus-0.1.jar"
                File "utils\schedworker\dist\lib\commons-cli-1.1.jar"
                File "utils\schedworker\dist\lib\commons-codec-1.6.jar"
                File "utils\schedworker\dist\lib\commons-collections-3.2.jar"
                File "utils\schedworker\dist\lib\commons-discovery-0.4.jar"
                File "utils\schedworker\dist\lib\commons-httpclient-3.1.jar"
                File "utils\schedworker\dist\lib\commons-io-2.4.jar"
                File "utils\schedworker\dist\lib\commons-lang3-3.1.jar"
                File "utils\schedworker\dist\lib\commons-logging-1.1.1.jar"
                File "utils\schedworker\dist\lib\commons-net-1.4.1.jar"
                File "utils\schedworker\dist\lib\commons-vfs-patched-2.0-SNAPSHOTr19199-tests.jar"
                File "utils\schedworker\dist\lib\commons-vfs-patched-2.0-SNAPSHOTr19199.jar"
                File "utils\schedworker\dist\lib\configadmin-1.2.4.jar"
                File "utils\schedworker\dist\lib\cron4j-rv2.2.3-mod.jar"
                File "utils\schedworker\dist\lib\cryptix-asn1.jar"
                File "utils\schedworker\dist\lib\cryptix.jar"
                File "utils\schedworker\dist\lib\cryptix32.jar"
                File "utils\schedworker\dist\lib\cxf-2.5.2.jar"
                File "utils\schedworker\dist\lib\derby.jar"
                File "utils\schedworker\dist\lib\derbytools.jar"
                File "utils\schedworker\dist\lib\dom4j-1.6.1.jar"
                File "utils\schedworker\dist\lib\dtdparser-1.21.jar"
                File "utils\schedworker\dist\lib\felix.jar"
                File "utils\schedworker\dist\lib\fluent-hc-4.2.1.jar"
                File "utils\schedworker\dist\lib\fractal-adl-2.7-SNAPSHOT.jar"
                File "utils\schedworker\dist\lib\fractal-api-2.0.3-SNAPSHOT.jar"
                File "utils\schedworker\dist\lib\fractal-gcm-api-1.1.1.jar"
                File "utils\schedworker\dist\lib\glite-wms-jdlj.jar"
                File "utils\schedworker\dist\lib\groovy-all-2.1.5.jar"
                File "utils\schedworker\dist\lib\h2-1.3.167.jar"
                File "utils\schedworker\dist\lib\hadoop-0.20.2-core.jar"
                File "utils\schedworker\dist\lib\hibernate-c3p0-4.1.3.Final.jar"
                File "utils\schedworker\dist\lib\hibernate-commons-annotations-4.0.1.Final.jar"
                File "utils\schedworker\dist\lib\hibernate-core-4.1.3.Final.jar"
                File "utils\schedworker\dist\lib\hibernate-jpa-2.0-api-1.0.1.Final.jar"
                File "utils\schedworker\dist\lib\http-2.0.4.jar"
                File "utils\schedworker\dist\lib\httpclient-4.2.1.jar"
                File "utils\schedworker\dist\lib\httpcore-4.2.1.jar"
                File "utils\schedworker\dist\lib\ibis-1.4.jar"
                File "utils\schedworker\dist\lib\ibis-connect-1.0.jar"
                File "utils\schedworker\dist\lib\ibis-util-1.0.jar"
                File "utils\schedworker\dist\lib\isorelax.jar"
                File "utils\schedworker\dist\lib\j-interop.jar"
                File "utils\schedworker\dist\lib\j-interopdeps.jar"
                File "utils\schedworker\dist\lib\jackrabbit-jcr-commons-1.5.2.jar"
                File "utils\schedworker\dist\lib\jackrabbit-webdav-1.5.2.jar"
                File "utils\schedworker\dist\lib\jakarta-oro-2.0.8.jar"
                File "utils\schedworker\dist\lib\java-xmlbuilder-0.4.jar"
                File "utils\schedworker\dist\lib\javassist-3.15.0-GA.jar"
                File "utils\schedworker\dist\lib\javassist.jar"
                File "utils\schedworker\dist\lib\jaxb-api-2.2.3.jar"
                File "utils\schedworker\dist\lib\jaxb-impl-2.2.4-1.jar"
                File "utils\schedworker\dist\lib\jaxb-xjc-2.2.4-1.jar"
                File "utils\schedworker\dist\lib\jaxrpc.jar"
                File "utils\schedworker\dist\lib\jaxrs-api-2.2.2.GA.jar"
                File "utils\schedworker\dist\lib\jboss-logging-3.1.0.GA.jar"
                File "utils\schedworker\dist\lib\jboss-transaction-api_1.1_spec-1.0.0.Final.jar"
                File "utils\schedworker\dist\lib\jcifs-1.2.19.jar"
                File "utils\schedworker\dist\lib\jcommon-1.0.6.jar"
                File "utils\schedworker\dist\lib\jdbm-2_4.jar"
                File "utils\schedworker\dist\lib\jdom.jar"
                File "utils\schedworker\dist\lib\jets3t-0.9.0.jar"
                File "utils\schedworker\dist\lib\jets3t-gui-0.9.0.jar"
                File "utils\schedworker\dist\lib\jettison-1.3.1.jar"
                File "utils\schedworker\dist\lib\jetty-6.1.18.jar"
                File "utils\schedworker\dist\lib\jetty-util-6.1.18.jar"
                File "utils\schedworker\dist\lib\jfreechart-1.0.2.jar"
                File "utils\schedworker\dist\lib\jna-platform-3.3.0.jar"
                File "utils\schedworker\dist\lib\jna.jar"
                File "utils\schedworker\dist\lib\jruby.jar"
                File "utils\schedworker\dist\lib\jsch-0.1.31.jar"
                File "utils\schedworker\dist\lib\jsch-0.1.38.jar"
                File "utils\schedworker\dist\lib\julia-asm-2.5.3-SNAPSHOT.jar"
                File "utils\schedworker\dist\lib\julia-mixins-2.5.3-SNAPSHOT.jar"
                File "utils\schedworker\dist\lib\julia-runtime-2.5.3-SNAPSHOT.jar"
                File "utils\schedworker\dist\lib\jython-2.5.4-rc1.jar"
                File "utils\schedworker\dist\lib\log4j.jar"
                File "utils\schedworker\dist\lib\mail.jar"
                File "utils\schedworker\dist\lib\mchange-commons-java-0.2.2.jar"
                File "utils\schedworker\dist\lib\moduleloader.jar"
                File "utils\schedworker\dist\lib\mofcomp.jar"
                File "utils\schedworker\dist\lib\msv.jar"
                File "utils\schedworker\dist\lib\mysql-connector-java-5.1.16-bin.jar"
                File "utils\schedworker\dist\lib\neethi-3.0.1.jar"
                File "utils\schedworker\dist\lib\netty-3.2.0.ALPHA2.jar"
                File "utils\schedworker\dist\lib\opensaml-1.1.jar"
                File "utils\schedworker\dist\lib\osgi.jar"
                File "utils\schedworker\dist\lib\puretls.jar"
                File "utils\schedworker\dist\lib\rabbitmq-client.jar"
                File "utils\schedworker\dist\lib\rampart-core-1.3.jar"
                File "utils\schedworker\dist\lib\rampart-policy-1.3.jar"
                File "utils\schedworker\dist\lib\rampart-trust-1.3.jar"
                File "utils\schedworker\dist\lib\relaxngDatatype.jar"
                File "utils\schedworker\dist\lib\rngpack-1.1a.jar"
                File "utils\schedworker\dist\lib\rrd4j-2.0.6.jar"
                File "utils\schedworker\dist\lib\saaj-impl.jar"
                File "utils\schedworker\dist\lib\servicebinder-0.9.0.jar"
                File "utils\schedworker\dist\lib\servlet-api-2.5-6.1.11.jar"
                File "utils\schedworker\dist\lib\servlet-api.jar"
                File "utils\schedworker\dist\lib\servlet.jar"
                File "utils\schedworker\dist\lib\shell-1.4.2.jar"
                File "utils\schedworker\dist\lib\shellgui.jar"
                File "utils\schedworker\dist\lib\shellplugin.jar"
                File "utils\schedworker\dist\lib\shelltui-1.4.2.jar"
                File "utils\schedworker\dist\lib\sigar\sigar-amd64-winnt.dll"
                File "utils\schedworker\dist\lib\sigar\sigar-x86-winnt.dll"
                File "utils\schedworker\dist\lib\sigar\sigar-x86-winnt.lib"
                File "utils\schedworker\dist\lib\sigar\sigar.jar"
                File "utils\schedworker\dist\lib\simple.jar"
                File "utils\schedworker\dist\lib\slf4j-api-1.5.6.jar"
                File "utils\schedworker\dist\lib\slf4j-log4j12-1.5.3.jar"
                File "utils\schedworker\dist\lib\slf4j-log4j12-1.5.6.jar"
                File "utils\schedworker\dist\lib\soap.jar"
                File "utils\schedworker\dist\lib\spring-aop-3.0.6.RELEASE.jar"
                File "utils\schedworker\dist\lib\spring-asm-3.0.6.RELEASE.jar"
                File "utils\schedworker\dist\lib\spring-beans-3.0.6.RELEASE.jar"
                File "utils\schedworker\dist\lib\spring-context-3.0.6.RELEASE.jar"
                File "utils\schedworker\dist\lib\spring-core-3.0.6.RELEASE.jar"
                File "utils\schedworker\dist\lib\spring-expression-3.0.6.RELEASE.jar"
                File "utils\schedworker\dist\lib\spring-web-3.0.6.RELEASE.jar"
                File "utils\schedworker\dist\lib\ssj.jar"
                File "utils\schedworker\dist\lib\stax2-api-3.1.1.jar"
                File "utils\schedworker\dist\lib\sunxacml.jar"
                File "utils\schedworker\dist\lib\synchronize-0.9.0.jar"
                File "utils\schedworker\dist\lib\tablelayout.jar"
                File "utils\schedworker\dist\lib\task-deployment-2.4-SNAPSHOT.jar"
                File "utils\schedworker\dist\lib\task-framework-2.4-SNAPSHOT.jar"
                File "utils\schedworker\dist\lib\trilead-ssh2.jar"
                File "utils\schedworker\dist\lib\typica.jar"
                File "utils\schedworker\dist\lib\uploader-0.9.0.jar"
                File "utils\schedworker\dist\lib\vfs_s3.jar"
                File "utils\schedworker\dist\lib\vi-java.jar"
                File "utils\schedworker\dist\lib\vim.jar"
                File "utils\schedworker\dist\lib\vim25.jar"
                File "utils\schedworker\dist\lib\virtualbox-wsdl.jar"
                File "utils\schedworker\dist\lib\virtualization-core.jar"
                File "utils\schedworker\dist\lib\virtualization-hyperv-winrm.jar"
                File "utils\schedworker\dist\lib\virtualization-hyperv-wmi.jar"
                File "utils\schedworker\dist\lib\virtualization-libvirt.jar"
                File "utils\schedworker\dist\lib\virtualization-virtualbox.jar"
                File "utils\schedworker\dist\lib\virtualization-vmware-vi.jar"
                File "utils\schedworker\dist\lib\virtualization-vmware-vix.jar"
                File "utils\schedworker\dist\lib\virtualization-xenserver.jar"
                File "utils\schedworker\dist\lib\wbem.jar"
                File "utils\schedworker\dist\lib\winp-1.17-patched.jar"
                File "utils\schedworker\dist\lib\winp.dll"
                File "utils\schedworker\dist\lib\winp.x64.dll"
                File "utils\schedworker\dist\lib\wiseman-tools.jar"
                File "utils\schedworker\dist\lib\woodstox-core-lgpl-4.1.2.jar"
                File "utils\schedworker\dist\lib\ws-commons-util-1.0.2.jar"
                File "utils\schedworker\dist\lib\wsdl4j-1.6.2.jar"
                File "utils\schedworker\dist\lib\wss4j-1.5.3.jar"
                File "utils\schedworker\dist\lib\xenserver-5.0.0-3.jar"
                File "utils\schedworker\dist\lib\xercesImpl-2.8.1.jar"
                File "utils\schedworker\dist\lib\xmlrpc-client-3.1.jar"
                File "utils\schedworker\dist\lib\xmlrpc-common-3.1.jar"
                File "utils\schedworker\dist\lib\xmlschema-core-2.0.1.jar"
                File "utils\schedworker\dist\lib\xmlsec-1.4.0.jar"
                File "utils\schedworker\dist\lib\xsdlib.jar"

                ; Depending on the architecture include the correct jre files
                !ifdef STANDALONE_X86
                       !insertmacro Log "Installing x86 jre into $INSTDIR\jre ..."
                       !include utils\x86\install_jre_x86.nsi
                !endif
                !ifdef STANDALONE_X64
                       !insertmacro Log "Installing x64 jre into $INSTDIR\jre ..."
                       !include utils\x64\install_jre_x64.nsi
                !endif
        !endif

        !insertmacro Log "Successfully copied files ..."

        ${If} $AllowEveryone == "1"
          !insertmacro Log "Allowing everyone to control the agent ..."
          !insertmacro Log "Granting members of ALL USERS group to start/stop the service ..."
          nsExec::Exec '"$INSTDIR\SetACL.exe" -log "${SETACL_LOG_PATH}" -on ${SERVICE_NAME} -ot srv -actn ace -ace "n:${ALL_USERS_SID};p:start_stop;s:y"'
          !insertmacro Log "Granting members of ALL USERS group the full control of regkey to be able to change the config file ..."
          nsExec::Exec '"$INSTDIR\SetACL.exe" -log "${SETACL_LOG_PATH}" -on HKEY_LOCAL_MACHINE\Software\ProActiveAgent -ot reg -actn setprot -op "dacl:p_nc;sacl:p_nc" -actn ace -ace "n:${ALL_USERS_SID};p:full;s:y"'
          !insertmacro Log "Granting members of ALL USERS group the full control of the selected $ConfigDir\${CONFIG_NAME} config file ..."
          nsExec::Exec '"$INSTDIR\SetACL.exe" -log "${SETACL_LOG_PATH}" -on "$ConfigDir\${CONFIG_NAME}" -ot file -actn ace -ace "n:${ALL_USERS_SID};p:full;s:y"'
          !insertmacro Log "Granting members of ALL USERS group the full control of the $ConfigDir\${CONFIG_DAY_NAME} config file ..."
          nsExec::Exec '"$INSTDIR\SetACL.exe" -log "${SETACL_LOG_PATH}" -on "$ConfigDir\${CONFIG_DAY_NAME}" -ot file -actn ace -ace "n:${ALL_USERS_SID};p:full;s:y"'
          !insertmacro Log "Granting members of ALL USERS group the full control of the $ConfigDir\${CONFIG_NIGHT_NAME} config file ..."
          nsExec::Exec '"$INSTDIR\SetACL.exe" -log "${SETACL_LOG_PATH}" -on "$ConfigDir\${CONFIG_NIGHT_NAME}" -ot file -actn ace -ace "n:${ALL_USERS_SID};p:full;s:y"'
        ${EndIf}

        ${IfNot} ${Silent}
          Exec "$INSTDIR\AgentForAgent.exe" ; Run the Agent GUI
        ${EndIf}

        !insertmacro Log "Installed sucessfully, ready to start the ProActiveAgent service ..."

        ${If} ${Silent}
          ${keybd_event} ${VK_RETURN} 1 ; Simulate a keyboard event of the Return key to return to prompt
        ${EndIf}
SectionEnd

Section "ProActive ScreenSaver"
  Call InstallProActiveScreenSaver
SectionEnd

Section "Start Menu Shortcuts"
  Call CreateDesktopShortCuts
SectionEnd

UninstallText "This will uninstall ProActive Agent. Hit next to continue."

Section "Uninstall"
  Call un.ProActiveAgent
SectionEnd

#################################################################
# Uninstall the ProActive agent
#################################################################
Function un.ProActiveAgent
  ; Check user admin rights
  System::Call "kernel32::GetModuleHandle(t 'shell32.dll') i .s"
  System::Call "kernel32::GetProcAddress(i s, i 680) i .r0"
  System::Call "::$0() i .r0"
  DetailPrint "Check: Current user is admin? $0"
  StrCmp $0 '0' 0 +3
  MessageBox MB_OK "Administrator rights are required to uninstall the ProActive Agent." /SD IDOK
  Abort

  MessageBox MB_OKCANCEL "This will delete $INSTDIR and all subdirectories and files?" /SD IDOK IDOK DoUninstall
  Abort "Quiting the uninstall process"

  DoUnInstall:

  Call un.TerminateAgentForAgent ; Terminate agent gui

  stopServiceLABEL:
  !insertmacro SERVICE "stop" ${SERVICE_NAME} "" "un."
  !insertmacro SERVICE "status" ${SERVICE_NAME} "" "un."
  Pop $0
  ${If} $0 != "stopped"
    Goto stopServiceLABEL
  ${EndIf}
  !insertmacro SERVICE "delete" ${SERVICE_NAME} "" "un."

  ; Ask the user if he wants to keep the configuration files
  ; In silent mode, we remove everything
  MessageBox MB_YESNO "Delete configuration files from $INSTDIR\config ?" /SD IDYES IDNO keepConfigLabel
  SetOutPath "$INSTDIR\config"
  Delete ${CONFIG_NAME}
  Delete ${CONFIG_DAY_NAME}
  Delete ${CONFIG_NIGHT_NAME}
  SetOutPath $INSTDIR
  RMDir /r "$INSTDIR\config"

  keepConfigLabel:

  ${If} ${FileExists} "$INSTDIR\config"
    ; Erase everything except config dir by copying into temp dir
    CreateDirectory "$TEMP\ProActiveAgentUninstall"
    CopyFiles "$INSTDIR\config\*" "$TEMP\ProActiveAgentUninstall"
    RMDir /R $INSTDIR
    CreateDirectory "$INSTDIR\config"
    CopyFiles "$TEMP\ProActiveAgentUninstall\*" "$INSTDIR\config"
    RMDir /R "$TEMP\ProActiveAgentUninstall"
  ${Else}
    RMDir /R $INSTDIR
  ${EndIf}

  SetShellVarContext all ; For all users
  ; Remove the screen saver
  Delete "$SYSDIR\ProActiveSSaver.scr"
  RMDir /r "$SMPROGRAMS\ProActiveAgent"

  ; Delete regkey from uninstall
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\ProActiveAgent"
  ; Delete entry from auto start
  DeleteRegValue HKLM "Software\Microsoft\Windows\CurrentVersion\Run" "ProActiveAgent"
  DeleteRegKey HKLM "Software\ProActiveAgent"

  SetShellVarContext current ; reset to current user
FunctionEnd

Function un.TerminateAgentForAgent
  ; See: http://nsis.sourceforge.net/Find_and_Close_or_Terminate
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
    ; Try to kill using taskkill command
    nsExec::Exec 'cmd.exe /C \
        taskkill /f /im AgentForAgent.exe'
    Goto ask_user

  abort:
    Abort

  finished:
    Pop $2
    Pop $1
    Pop $0

FunctionEnd

#################################################################
# Locate the java home dir from registry, the result value
# is stored in $0
#################################################################
Function LocateJava
        ReadEnvStr $0 JAVA_HOME
        ${If} $0 != ""
          !insertmacro Log "Reading JAVA_HOME environment variable ..."
          Goto javaFoundLabel
        ${EndIf}

        ${If} ${RunningX64}
            !insertmacro Log "Using x64 regview ..."
            SetRegView 64
            ReadRegStr $0 HKLM "SOFTWARE\JavaSoft\Java Development Kit" "CurrentVersion"
            ${If} $0 != ""
              !insertmacro Log "Locating java jdk home ..."
              ReadRegStr $0 HKLM "SOFTWARE\JavaSoft\Java Development Kit\$0" "JavaHome"
              SetRegView 32
              Goto javaFoundLabel
            ${EndIf}
            SetRegView 32
        ${EndIf}

        ReadRegStr $0 HKLM "SOFTWARE\JavaSoft\Java Development Kit" "CurrentVersion"
        ${If} $0 != ""
          !insertmacro Log "Locating java jdk home ..."
          ReadRegStr $0 HKLM "SOFTWARE\JavaSoft\Java Development Kit\$0" "JavaHome"
          Goto javaFoundLabel
        ${EndIf}

        ${If} ${RunningX64}
            !insertmacro Log "Using x64 regview ..."
            SetRegView 64
            ReadRegStr $0 HKLM "SOFTWARE\JavaSoft\Java Runtime Environment" "CurrentVersion"
            ${If} $0 != ""
              !insertmacro Log "Locating java jre home ..."
              ReadRegStr $0 HKLM "SOFTWARE\JavaSoft\Java Runtime Environment\$0" "JavaHome"
              SetRegView 32
              Goto javaFoundLabel
            ${EndIf}
            SetRegView 32
        ${EndIf}

        ReadRegStr $0 HKLM "SOFTWARE\JavaSoft\Java Runtime Environment" "CurrentVersion"
        ${If} $0 != ""
          !insertmacro Log "Locating java jre home ..."
          ReadRegStr $0 HKLM "SOFTWARE\JavaSoft\Java Runtime Environment\$0" "JavaHome"
          Goto javaFoundLabel
        ${EndIf}

        javaFoundLabel:
          Push $0
FunctionEnd

#################################################################
# GetCurrentTime
# Written by Saivert
# Description:
#   Uses System.dll plugin to call Win32's GetTimeFormat in order
#   to get the current time as a formatted string.
#################################################################
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

#################################################################
# GetCurrentDate
# Written by Saivert
# Description:
#   Uses System.dll plugin to call Win32's GetDateFormat in order
#   to get the current date as a formatted string.
#################################################################
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

#################################################################
# Taken from http://nsis.sourceforge.net/CharStrip_%26_StrStrip:_Remove_character_or_string_from_another_string
#################################################################
Function StrStrip
Exch $R0 ;string
Exch
Exch $R1 ;in string
Push $R2
Push $R3
Push $R4
Push $R5
 StrLen $R5 $R0
 StrCpy $R2 -1
 IntOp $R2 $R2 + 1
 StrCpy $R3 $R1 $R5 $R2
 StrCmp $R3 "" +9
 StrCmp $R3 $R0 0 -3
  StrCpy $R3 $R1 $R2
  IntOp $R2 $R2 + $R5
  StrCpy $R4 $R1 "" $R2
  StrCpy $R1 $R3$R4
  IntOp $R2 $R2 - $R5
  IntOp $R2 $R2 - 1
  Goto -10
  StrCpy $R0 $R1
Pop $R5
Pop $R4
Pop $R3
Pop $R2
Pop $R1
Exch $R0
FunctionEnd

#################################################################
# Replace in FILE
#################################################################
Function RIF
  ClearErrors  ; want to be a newborn

  Exch $0      ; REPLACEMENT
  Exch
  Exch $1      ; SEARCH_TEXT
  Exch 2
  Exch $2      ; SOURCE_FILE

  Push $R0     ; SOURCE_FILE file handle
  Push $R1     ; temporary file handle
  Push $R2     ; unique temporary file name
  Push $R3     ; a line to sar/save
  Push $R4     ; shift puffer

  IfFileExists $2 +1 RIF_error      ; knock-knock
  FileOpen $R0 $2 "r"               ; open the door

  GetTempFileName $R2               ; who's new?
  FileOpen $R1 $R2 "w"              ; the escape, please!

  RIF_loop:                         ; round'n'round we go
    FileRead $R0 $R3                ; read one line
    IfErrors RIF_leaveloop          ; enough is enough
    RIF_sar:                        ; sar - search and replace
      Push "$R3"                    ; (hair)stack
      Push "$1"                     ; needle
      Push "$0"                     ; blood
      Call StrRep                   ; do the bartwalk
      StrCpy $R4 "$R3"              ; remember previous state
      Pop $R3                       ; gimme s.th. back in return!
      StrCmp "$R3" "$R4" +1 RIF_sar ; loop, might change again!
    FileWrite $R1 "$R3"             ; save the newbie
  Goto RIF_loop                     ; gimme more

  RIF_leaveloop:                    ; over'n'out, Sir!
    FileClose $R1                   ; S'rry, Ma'am - clos'n now
    FileClose $R0                   ; me 2

    Delete "$2.old"                 ; go away, Sire
    Rename "$2" "$2.old"            ; step aside, Ma'am
    Rename "$R2" "$2"               ; hi, baby!

    ClearErrors                     ; now i AM a newborn
    Goto RIF_out                    ; out'n'away

  RIF_error:                        ; ups - s.th. went wrong...
    SetErrors                       ; ...so cry, boy!

  RIF_out:                          ; your wardrobe?
  Pop $R4
  Pop $R3
  Pop $R2
  Pop $R1
  Pop $R0
  Pop $2
  Pop $0
  Pop $1
FunctionEnd

!define StrRep "!insertmacro StrRep"
!macro StrRep output string old new
    Push "${string}"
    Push "${old}"
    Push "${new}"
    !ifdef __UNINSTALL__
        Call un.StrRep
    !else
        Call StrRep
    !endif
    Pop ${output}
!macroend

!macro Func_StrRep un
    Function ${un}StrRep
        Exch $R2 ;new
        Exch 1
        Exch $R1 ;old
        Exch 2
        Exch $R0 ;string
        Push $R3
        Push $R4
        Push $R5
        Push $R6
        Push $R7
        Push $R8
        Push $R9

        StrCpy $R3 0
        StrLen $R4 $R1
        StrLen $R6 $R0
        StrLen $R9 $R2
        loop:
            StrCpy $R5 $R0 $R4 $R3
            StrCmp $R5 $R1 found
            StrCmp $R3 $R6 done
            IntOp $R3 $R3 + 1 ;move offset by 1 to check the next character
            Goto loop
        found:
            StrCpy $R5 $R0 $R3
            IntOp $R8 $R3 + $R4
            StrCpy $R7 $R0 "" $R8
            StrCpy $R0 $R5$R2$R7
            StrLen $R6 $R0
            IntOp $R3 $R3 + $R9 ;move offset by length of the replacement string
            Goto loop
        done:

        Pop $R9
        Pop $R8
        Pop $R7
        Pop $R6
        Pop $R5
        Pop $R4
        Pop $R3
        Push $R0
        Push $R1
        Pop $R0
        Pop $R1
        Pop $R0
        Pop $R2
        Exch $R1
    FunctionEnd
!macroend
!insertmacro Func_StrRep ""
!insertmacro Func_StrRep "un."