@echo off

nuget restore ..\Projects\RubezhService\RubezhService.sln
nuget restore ..\Projects\RubezhAdministrator\RubezhAdministrator.sln
nuget restore ..\Projects\RubezhMonitor\RubezhMonitor.sln
nuget restore ..\Projects\GKOPCServer\GKOPCServer.sln

set BuildFilesPath=%cd%
set NAntPath=%cd%\..\3rdParty\NAnt\bin\NAnt.exe
del %cd%\bin\GK /Q
start /I %NAntPath% -logger:NAnt.Core.XmlLogger -logfile:buildlog.xml -buildfile:nant/BuildRelease.build
echo END