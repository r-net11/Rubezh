
MSBuild.exe /v:m /clp:ErrorsOnly Source\Version2.0\Xceed.Wpf.AvalonDock.sln /p:Configuration=Release /t:ReBuild /nologo

rmdir Bin\ /S /Q
xcopy Source\Version2.0\Xceed.Wpf.AvalonDock\bin\Release Bin\Version2.0\Xceed.Wpf.AvalonDock /E /K /Y /C /R /Q /I
xcopy Source\Version2.0\Xceed.Wpf.AvalonDock.Themes.Aero\bin\Release Bin\Version2.0\Xceed.Wpf.AvalonDock.Themes.Aero /E /K /Y /C /R /Q /I
xcopy Source\Version2.0\Xceed.Wpf.AvalonDock.Themes.Expression\bin\Release Bin\Version2.0\Xceed.Wpf.AvalonDock.Themes.Expression /E /K /Y /C /R /Q /I
xcopy Source\Version2.0\Xceed.Wpf.AvalonDock.Themes.Metro\bin\Release Bin\Version2.0\Xceed.Wpf.AvalonDock.Themes.Metro /E /K /Y /C /R /Q /I
xcopy Source\Version2.0\Xceed.Wpf.AvalonDock.Themes.VS2010\bin\Release Bin\Version2.0\Xceed.Wpf.AvalonDock.Themes.VS2010 /E /K /Y /C /R /Q /I

pause