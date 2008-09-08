@echo off

rem if number of inner params > 1 they should be surrounded by double-quotes

rem if optional and unused then pass "" as a parameter
rem 1st param - PROACTIVE DIRECTORY obligatory
rem 2nd param - JVM LOCATION optional
rem 3rd param - JVM PARAMS optional
rem 4th param - ACTION CMD obligatory
rem 5th param - ACTION PARAMS optional

SET PROACTIVE=
SET JAVA_PARAMs=""
SET ACTION_CMD=
SET ACTION_PARAMS=""
SET HOME_DIR=""
SET LOG4J_OPTION = ""

set PROACTIVE=%1
if %2 NEQ "" (set JAVA_HOME=%2)
if %3 NEQ "" (set JAVA_PARAMS=%3)
set ACTION_CMD=%4
if %5 NEQ "" (set ACTION_PARAMS=%5)

SETLOCAL ENABLEDELAYEDEXPANSION enabledelayedexpansion


rem unquote params if necessary

set PROACTIVE=%PROACTIVE:"=%
set JAVA_HOME=%JAVA_HOME:"=%
set JAVA_PARAMS=%JAVA_PARAMS:"=%
set ACTION_CMD=%ACTION_CMD:"=%
set ACTION_PARAMS=%ACTION_PARAMS:"=%
set HOME_DIR=%~dp0
set LOG4J_OPTION = ""

set PA_SCHEDULER=%PROACTIVE%


echo ProActiveDir: %PROACTIVE%
echo JDKHome     : %JAVA_HOME%
echo JDKParams   : %JAVA_PARAMS%
echo ActionCMD   : %ACTION_CMD%
echo ActionParams: %ACTION_PARAMS%

IF EXIST "%PROACTIVE%\bin\init.bat" (
   call "%PROACTIVE%\bin\init.bat"
   ) ELSE (
   call "%PROACTIVE%\bin\windows\init.bat" )

 echo %CLASSPATH%
rem %JAVA_CMD% %JAVA_PARAMS% org.objectweb.proactive.p2p.daemon.PAAgentServiceStarter %ACTION_CMD% %ACTION_PARAMS%

rem LOG
IF EXIST "%HOME_DIR%proactive-log4j" (
	set LOG4J_OPTION=-Dlog4j.configuration="file:///%HOME_DIR%proactive-log4j"
	rem set LOG4J_OPTION=-Dlog4j.configuration=
)
echo %LOG4J_OPTION%

rem PORT
IF EXIST "%HOME_DIR%proactive-log4j" (
	set LOG4J_OPTION=-Dlog4j.configuration="file:///%HOME_DIR%proactive-log4j"
	rem set LOG4J_OPTION=-Dlog4j.configuration=
)
-Dproactive.rmi.port=4555

if "%ACTION_CMD%" == "P2P" (%JAVA_CMD% %LOG4J_OPTION% %JAVA_PARAMS% org.objectweb.proactive.extra.p2p.daemon.PAAgentServiceP2PStarter %ACTION_PARAMS%)
if "%ACTION_CMD%" == "RM" (%JAVA_CMD% %LOG4J_OPTION% %JAVA_PARAMS% org.ow2.proactive.resourcemanager.utils.PAAgentServiceRMStarter %ACTION_PARAMS%)
rem if "%ACTION_CMD%" == "ADVERT" (%JAVA_CMD% -Dlog4j.configuration="file:///C:\Documents and Settings\Administrateur\.proactive\proactive-log4j" %JAVA_PARAMS% org.objectweb.proactive.core.util.winagent.PAAgentServiceRMIStarter %ACTION_PARAMS%)
if "%ACTION_CMD%" == "ADVERT" (%JAVA_CMD% %LOG4J_OPTION% %JAVA_PARAMS% org.objectweb.proactive.core.util.winagent.PAAgentServiceRMIStarter %ACTION_PARAMS%)
endlocal
