@echo off
set BuildFilesPath=%cd%
set NAntPath=%cd%\..\3rdParty\NAnt\bin\NAnt.exe
del %cd%\bin\Resurs /Q
start /I %NAntPath% -logger:NAnt.Core.XmlLogger -logfile:buildlog.xml -buildfile:nant/BuildResursRelease.build
echo END