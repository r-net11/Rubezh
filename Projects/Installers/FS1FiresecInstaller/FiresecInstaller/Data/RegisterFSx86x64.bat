@Echo off
if defined ProgramFiles(x86) goto INS64
goto INS32

:INS32
"C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\regtlibv12.exe" "%ProgramFiles%\Firesec5\lib\fs_types.dll"
regsvr32.exe "%ProgramFiles%\Firesec5\lib\fs_types.dll"
regsvr32.exe "%ProgramFiles%\Firesec5\lib\SockProxy.dll"
"%ProgramFiles%\Firesec5\fs_server.exe" -regserver
goto END

:INS64
"C:\WINDOWS\Microsoft.NET\Framework\v4.0.30319\regtlibv12.exe" "%ProgramFiles(x86)%\Firesec5\lib\fs_types.dll"
regsvr32.exe "%ProgramFiles(x86)%\Firesec5\lib\fs_types.dll"
regsvr32.exe "%ProgramFiles(x86)%\Firesec5\lib\SockProxy.dll"
"%ProgramFiles(x86)%\Firesec5\fs_server.exe" -regserver
goto END

:END

