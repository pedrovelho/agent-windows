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

rem echo %CLASSPATH%

if "%ACTION_CMD%" == "P2P" (%JAVA_CMD% %JAVA_PARAMS% org.objectweb.proactive.extra.p2p.daemon.PAAgentServiceP2PStarter %ACTION_PARAMS%)
if "%ACTION_CMD%" == "RM" (%JAVA_CMD% %JAVA_PARAMS% org.ow2.proactive.resourcemanager.utils.PAAgentServiceRMStarter %ACTION_PARAMS%)
if "%ACTION_CMD%" == "ADVERT" (%JAVA_CMD% %JAVA_PARAMS% org.objectweb.proactive.core.util.winagent.PAAgentServiceRMIStarter %ACTION_PARAMS%)

endlocal
