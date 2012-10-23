@echo off
set admPath="%cd%\..\..\..\..\FireAdministrator\bin\Debug\Logs"
set destAdm="%cd%\Logs %date%\FireAdministrator\"
set monPath="%cd%\..\..\..\..\FireMonitor\bin\Debug\Logs"
set destMon="%cd%\Logs %date%\FireMonitor\"
set servicePath="%cd%\..\..\..\..\FiresecService\bin\Debug\Logs"
set destService="%cd%\Logs %date%\FiresecService\"
set servicePath="%cd%\..\..\..\..\FiresecOPCServer\bin\Debug\Logs"
set destService="%cd%\Logs %date%\FiresecOPCServer\"
echo %admPath%
xcopy %admPath% %destAdm% /E /Q /K /Y
xcopy %monPath% %destMon% /E /Q /K /Y
xcopy %servicePath% %destService% /E /Q /K /Y
PAUSE
echo END