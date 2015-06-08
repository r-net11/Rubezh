@echo off
for /f "tokens=*" %%i in (update_cppwrapper.lst) do xcopy /s /y %%i "Redistributables_1\"
for /f "tokens=*" %%i in (update_cppwrapper.lst) do xcopy /s /y %%i "Redistributables_1.new\"
for /f "tokens=*" %%i in (update_cppwrapper.lst) do xcopy /s /y %%i "ChinaSKDDriver\bin\Debug\"
for /f "tokens=*" %%i in (update_cppwrapper.lst) do xcopy /s /y %%i "ChinaSKDDriverTest\bin\Debug\"
for /f "tokens=*" %%i in (update_cppwrapper.lst) do xcopy /s /y %%i "..\FiresecService\bin\Debug\"
