@echo off
set dataPath=%cd%
set iconsPath=%cd%\Icons
set deviceLibraryPath=%cd%\DeviceLibrary.xml
set plansConfig=%cd%\PlansConfiguration.xml
set securityConfig=%cd%\SecurityConfiguration.xml
chdir ..
chdir FiresecService\FiresecService\bin\Debug 
set destinationSoundPath=%cd%\Configuration\Sounds\
set destinationIconsPath=%cd%\Configuration\Icons\
set dest=%cd%\Configuration\
xcopy %iconsPath% %destinationIconsPath% /E /Q /K /Y
xcopy "C:\Program Files\Firesec\Icons\*.*" %destinationIconsPath% /E /Q /K /Y
xcopy "C:\Program Files\Firesec\Sounds\*.*" %destinationSoundPath% /E /Q /K /Y
xcopy %plansConfig% %dest% /E /Q /K /Y
xcopy %deviceLibraryPath% %dest% /E /Q /K /Y
xcopy %securityConfig% %dest% /E /Q /K /Y

