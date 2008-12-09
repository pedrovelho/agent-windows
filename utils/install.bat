@ECHO OFF

REM The following directory is for .NET 2.0
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v2.0.50727
set PATH=%PATH%;%DOTNETFX2%

echo Installing WindowsService...
echo ---------------------------------------------------
InstallUtil /i /LogToConsole=false /user=%1 /pass=%2 /domain=%3 "ProActiveAgent.exe"
echo ---------------------------------------------------
echo Done.