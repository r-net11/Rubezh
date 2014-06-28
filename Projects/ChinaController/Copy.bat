@echo off
xcopy /s /y "Redistributables_1\*" "OriginalSDK\EntranceGuardDemo\Bin\"
xcopy /s /y "Redistributables_1\*" "OriginalSDK\EventAttach\Bin\"
xcopy /s /y "Redistributables_1\*" "CPPWrapper\Bin\"
xcopy /s /y "Redistributables_1\*" "ChilnaSKDDriver\bin\Debug\"
xcopy /s /y "Redistributables_1\*" "ChilnaSKDDriverTest\bin\Debug\"
xcopy /s /y "Redistributables_1\*" "ChilnaSKDDriverConsoleTest\bin\Debug\"
xcopy /s /y "Redistributables_1\*" "..\FiresecService\bin\Debug\"
xcopy /s /y "Redistributables_1\*" "..\FiresecService\bin\Release\"
