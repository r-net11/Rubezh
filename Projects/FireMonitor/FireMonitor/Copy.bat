@echo off
chdir bin\Debug
set destinationPath=%cd%\Data\
chdir ..
chdir ..
chdir ..
chdir ..
chdir Data
set dataPath=%cd%
xcopy %dataPath% %destinationPath% /E /Q /K