@echo off
set BuildFilesPath=%cd%
set NAntPath=%cd%\..\3rdParty\NAnt\bin\NAnt.exe
%NAntPath% -logger:NAnt.Core.XmlLogger -logfile:buildlog_Copy3dPartyDlls.xml -buildfile:nant/Copy3dPartyDlls.build
echo END