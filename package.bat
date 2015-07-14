@echo off

call build.bat release

nuget pack ftreplace\ftreplace.nuspec -o build
