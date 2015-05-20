@echo off
rem xcopy /s /y "Redistributables_1\*" "OriginalSDK\EntranceGuardDemo\Bin\"
rem xcopy /s /y "Redistributables_1\*" "OriginalSDK\EventAttach\Bin\"
xcopy /s /y "Redistributables_1\*" "CPPWrapper\Bin\"
xcopy /s /y "Redistributables_1\*" "ChinaSKDDriver\bin\Debug\"
xcopy /s /y "Redistributables_1\*" "ChinaSKDDriverTest\bin\Debug\"
xcopy /s /y "Redistributables_1\*" "..\FiresecService\bin\Debug\"
xcopy /s /y "Redistributables_1\*" "..\FiresecService\bin\Release\"
