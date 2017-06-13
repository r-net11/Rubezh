@echo off
set BuildFilesPath=%cd%
set NAntPath=%cd%\..\3rdParty\NAnt\bin\NAnt.exe
del %cd%\bin\GK /Q
%NAntPath% -logger:NAnt.Core.XmlLogger -logfile:buildlog_Copy3dPartyDlls.xml -buildfile:nant/Copy3dPartyDlls.build
%NAntPath% -logger:NAnt.Core.XmlLogger -logfile:buildlog_CopySDK.xml -buildfile:nant/CopySDK.build
%NAntPath% -logger:NAnt.Core.XmlLogger -logfile:buildlog_BuildRelease.xml -buildfile:nant/BuildRelease.build -D:InstallerAssemblyNamePrefix= -D:InstallerAssemblyName=A.C.Tech -D:OurProductVersion=1.0.3 -D:OurProductBuildNumber=6 -D:Culture=ru
echo END