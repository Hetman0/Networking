@ECHO OFF
SETLOCAL

SET cscDir=%WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe

ECHO Using %cscDir%
ECHO.

SET CONS=/recurse:ConApp.cs
SET APPL=/recurse:WinApp.cs
SET LIBR=/recurse:abstract\*.cs attribute\*.cs delegate\*.cs enum\*.cs exception\*.cs instance\*.cs interface\*.cs static\*.cs struct\*.cs

SET conDir=%cd%\bin\console
if not exist %conDir% mkdir %conDir%
%cscDir% /nowarn:1591 /doc:%conDir%\doc.xml /out:%conDir%\bin.exe /target:exe /main:ConApp %CONS% %LIBR%
REM start %conDir%\bin.exe

REM SET appDir=%cd%\bin\application
REM if not exist %appDir% mkdir %appDir%
REM %cscDir% /nowarn:1591 /doc:%appDir%\doc.xml /out:%appDir%\bin.exe /target:winexe /main:WinApp %APPL% %LIBR%

REM SET libDir=%cd%\bin\library
REM if not exist %libDir% mkdir %libDir%
REM %cscDir% /nowarn:1591 /doc:%libDir%\doc.xml /out:%libDir%\bin.exe /target:library %LIBR%

ENDLOCAL
@ECHO ON