@echo off
set connection=%~1
set password=%~2
set mode=%~3

IF "%mode%"=="vsts" (
	set package_root=..\..\
) ELSE (
	set package_root=%CD%
)

REM Find the spkl in the package folder (irrespective of version)
For /R %package_root% %%G IN (spkl.exe) do (
	IF EXIST "%%G" (set spkl_path=%%G
	goto :continue)
	)

:continue

@echo Using spkl from '%spkl_path%'
REM spkl plugins [path] [connection-string] [/p:release]
"%spkl_path%" plugins Plugins "%connection%%password%" /p:release

if errorlevel 1 (
	echo Error Code=%errorlevel%
	exit /b %errorlevel%
)

"%spkl_path%" webresources WebResources "%connection%%password%"

if errorlevel 1 (
	echo Error Code=%errorlevel%
	exit /b %errorlevel%
)