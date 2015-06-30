DEL /F/S/Q Bin\

"c:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" /v:m /clp:ErrorsOnly Source\CodeReason.Reports.VS2008.sln /p:Configuration=Release

MKDIR Bin
COPY Source\CodeReason.Reports\bin\Release\CodeReason.Reports.dll Bin\
COPY Source\CodeReason.Reports\bin\Release\CodeReason.Reports.pdb Bin\
COPY Source\CodeReason.Reports.Charts.Visifire\bin\Release\CodeReason.Reports.Charts.Visifire.dll Bin\
COPY Source\CodeReason.Reports.Charts.Visifire\bin\Release\WPFVisifire.Charts.dll Bin\
COPY Source\CodeReason.Reports.Charts.Visifire\bin\Release\CodeReason.Reports.Charts.Visifire.pdb Bin\

pause