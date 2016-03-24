@echo off
cd /d %~dp0
call msbuild FiresecService\FiresecService.sln /t:Clean /p:Configuration=Debug /verbosity:n /m
call msbuild FireAdministrator\FireAdministrator.sln /t:Clean /p:Configuration=Debug /verbosity:n /m
call msbuild FireMonitor\FireMonitor.sln /t:Clean /p:Configuration=Debug /verbosity:n /m
call msbuild GKImitator\GKImitator.sln /t:Clean /p:Configuration=Debug /verbosity:n /m
cls
nuget restore FiresecService\FiresecService.sln
call msbuild FiresecService\FiresecService.sln /t:Rebuild /p:Configuration=Debug /verbosity:n /m
cls
nuget restore FireAdministrator\FireAdministrator.sln
call msbuild FireAdministrator\FireAdministrator.sln /t:Rebuild /p:Configuration=Debug /verbosity:n /m
cls
nuget restore FireMonitor\FireMonitor.sln
call msbuild FireMonitor\FireMonitor.sln /t:Rebuild /p:Configuration=Debug /verbosity:n /m
cls
nuget restore GKImitator\GKImitator.sln
call msbuild GKImitator\GKImitator.sln /t:Rebuild /p:Configuration=Debug /verbosity:n /m
timeout /t -1

