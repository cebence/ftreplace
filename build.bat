@echo off

rem Set build parameters.
set _CONFIG_=/p:Configuration=Debug
set _PLATFORM_=

rem Use .NET Framework SDK 4.5 by default.
set TARGET_SDK=

rem But if there's 4.5.1 use that one instead.
if exist "C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.1" set TARGET_SDK=/p:TargetFrameworkVersion=v4.5.1

msbuild ftreplace.sln %_CONFIG_% %_PLATFORM_% %TARGET_SDK%
