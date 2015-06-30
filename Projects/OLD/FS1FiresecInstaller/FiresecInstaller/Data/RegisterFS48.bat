@Echo off
if defined ProgramFiles(x86) goto INS64
goto INS32

:INS32
"C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\regtlibv12.exe" "%ProgramFiles%\Firesec\fs_types.dll" /S
regsvr32.exe "%ProgramFiles%\Firesec\fs_types.dll" /S
regsvr32.exe "%ProgramFiles%\Firesec\SockProxy.dll" /S
"%ProgramFiles%\Firesec\fs_server.exe" -regserver /S
goto END

:INS64
"C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\regtlibv12.exe" "%ProgramFiles(x86)%\Firesec\fs_types.dll" /S
regsvr32.exe "%ProgramFiles(x86)%\Firesec\fs_types.dll" /S
regsvr32.exe "%ProgramFiles(x86)%\Firesec\SockProxy.dll" /S
"%ProgramFiles(x86)%\Firesec\fs_server.exe" -regserver /S
goto END

:END

