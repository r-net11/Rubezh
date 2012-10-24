@echo off
set admPath="%cd%\..\FireAdministrator\Logs"
set destAdm="%cd%\Logs %date%\FireAdministrator\"
set monPath="%cd%\..\FireMonitor\Logs"
set destMon="%cd%\Logs %date%\FireMonitor\"
set servicePath="%cd%\..\FiresecService\Logs"
set destService="%cd%\Logs %date%\FiresecService\"
set servicePath="%cd%\..\FiresecOPCServer\Logs"
set destService="%cd%\Logs %date%\FiresecOPCServer\"
echo %admPath%
xcopy %admPath% %destAdm% /E /Q /K /Y
xcopy %monPath% %destMon% /E /Q /K /Y
xcopy %servicePath% %destService% /E /Q /K /Y
PAUSE
echo END