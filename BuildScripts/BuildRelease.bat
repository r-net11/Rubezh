@echo off

nuget restore ..\Projects\FiresecService\FiresecService.sln
nuget restore ..\Projects\FireAdministrator\FireAdministrator.sln
nuget restore ..\Projects\FireMonitor\FireMonitor.sln
nuget restore ..\Projects\FireMonitor\FireMonitor.sln

set BuildFilesPath=%cd%
set NAntPath=%cd%\..\3rdParty\NAnt\bin\NAnt.exe
del %cd%\bin\GK /Q
start /I %NAntPath% -logger:NAnt.Core.XmlLogger -logfile:buildlog.xml -buildfile:nant/BuildRelease.build
echo END