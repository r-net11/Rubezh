REM Находим opcproxy.dll и импортируем из неё типы данных COM в .NET
REM при помощи утилиты TlbImp.exe

REM Переходим в директорию TlbImp.exe
CD /D "C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\"
REM Конвертируем...
TlbImp.exe C:\Windows\SysWOW64\opcproxy.dll /out:D:\opcproxy.dll
PAUSE
