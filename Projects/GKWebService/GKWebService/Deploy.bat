cd /d %~dp0
xcopy /Y /E App C:\inetpub\wwwroot\App\
xcopy /Y /E bin C:\inetpub\wwwroot\bin\
xcopy /Y /E Content C:\inetpub\wwwroot\Content\
xcopy /Y /E fonts C:\inetpub\wwwroot\fonts\
xcopy /Y /E Scripts C:\inetpub\wwwroot\Scripts\
xcopy /Y /E Views C:\inetpub\wwwroot\Views\
xcopy /Y GKWebService.wpp.targets C:\inetpub\wwwroot\
xcopy /Y Global.asax C:\inetpub\wwwroot\
xcopy /Y packages.config C:\inetpub\wwwroot\
xcopy /Y Web.config C:\inetpub\wwwroot\
pause