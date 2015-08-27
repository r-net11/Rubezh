@echo off
set BuildFilesPath=%cd%
set NAntPath=%cd%\..\3rdParty\NAnt\bin\NAnt.exe
%NAntPath% -logger:NAnt.Core.XmlLogger -logfile:buildlog_CopySDK.xml -buildfile:nant/CopySDK.build
echo END