@echo off

call build.bat release

nuget pack ftreplace\ftreplace.csproj -Prop Configuration=Release
