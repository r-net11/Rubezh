@echo off
set dataPath=%cd%
set soundsPath=%cd%\Sounds
chdir ..
chdir FiresecService\FiresecService\bin\Debug
set destinationSoundPath=%cd%\Sounds\
xcopy %soundsPath% %destinationSoundPath% /E /Q /K