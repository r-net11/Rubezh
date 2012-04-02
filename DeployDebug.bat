@echo off
set BuildFilesPath=%cd%
set NAntPath=%cd%\3rdParty\NAnt\bin\NAnt.exe
start /I %NAntPath% -buildfile:DeployDebug.build