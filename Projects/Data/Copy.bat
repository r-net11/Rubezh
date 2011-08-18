@echo off
set dataPath=%cd%
set soundsPath=%cd%\Sounds
set iconsPath=%cd%\Icons
chdir ..
chdir FiresecService\FiresecService\bin\Debug
set destinationSoundPath=%cd%\Sounds\
set destinationIconsPath=%cd%\Icons\
xcopy %iconsPath% %destinationIconsPath% /E /Q /K
xcopy %soundsPath% %destinationSoundPath% /E /Q /K