@echo off
set admPath="%cd%\..\FireAdministrator\Logs"
set destAdm="%cd%\FireAdministrator\"
set monPath="%cd%\..\FireMonitor\Logs"
set destMon="%cd%\FireMonitor\"
set servicePath="%cd%\..\FiresecService\Logs"
set destService="%cd%\FiresecService\"
echo %admPath%
xcopy %admPath% %destAdm% /E /Q /K /Y
xcopy %monPath% %destMon% /E /Q /K /Y
xcopy %servicePath% %destService% /E /Q /K /Y
PAUSE
echo END