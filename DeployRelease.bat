@echo off
set BuildFilesPath=%cd%
set NAntPath=%cd%\3rdParty\NAnt\bin\NAnt.exe
start /I %NAntPath% -logger:NAnt.Core.XmlLogger -logfile:buildlog.xml -buildfile:DeployRelease.build