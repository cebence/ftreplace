# File Text Replace (ftreplace) utility

[![Build status](https://ci.appveyor.com/api/projects/status/0reuf6rvv57crl25?svg=true)](https://ci.appveyor.com/project/cebence/ftreplace)
[![NuGet version](https://img.shields.io/nuget/v/echoer.svg)](https://www.nuget.org/packages/FileTextReplace/)

A simple command-line utility that came out of necessity to rewrite C# project's `App.config` and redirect `codebase` references into project's `Debug` folder.

The tool can copy a source (template) file to the destination location, and/or replace all occurrences of `find` string with the `replace` string in the destination file.

The tool will exit with a `0` exit code if everything was OK. Otherwise, if any of the arguments is missing and/or invalid, files are inaccessible etc. exit code will be `1`.

## Command-line arguments

- `-i <filename>` - Source filename, optional - if omitted equals destination filename.
- `-o <filename>` - Destination filename, required.
- `-f <string>` - Text to find (plain text, no regular expressions), optional.
- `-r <string>` - Replacement text (plain text, no regular expressions), optional.
- `--help` - Displays how the tool is supposed to be used.
- `--debug` - Displays all values (filenames and strings).

If you omit both `-f` and `-r` the tool will just copy the text from one file to another.
If you omit only one of them the tool will exit with an error.

> **Note:** If you are calling the tool from *Visual Studio* or *MSBuild*, and your find/replace text contains placeholders surrounded with `%`, e.g. `ftreplace -f %SOMETHING%`, **do not forget** to double the `%` like this `ftreplace -f %%SOMETHING%%`.
> It seems that MSBuild's `Exec` task is trying to resolve anything that resembles an environment variable, and you (actually the tool :smile:) would get an empty string.

## Usage examples

The `help` option overrides all other arguments, i.e. the following command will not do any processing:

```
ftreplace --help -i file.in -o file.out -f ME -r YOU
```

The following command will copy text from the `keys.txt` file, replace all occurrences of the case-sensitive word `KEY` with `LOCK`, and store the modified text to the `locks.txt` file.

```
ftreplace -i keys.txt -o locks.txt -f KEY -r LOCK
```

This command will replace all occurrences of the case-sensitive word `.dll` with `.exe` in the `MyApp.exe.config` file:

```
ftreplace -o MyApp.exe.config -f .dll -r .exe
```

The same command, when `debug` switch is ON, will produce the following output:

```
> ftreplace --debug -o MyApp.exe.config -f .dll -r .exe

IN: (same as OUT)
OUT: MyApp.exe.config
FIND: .dll
REPLACE: .exe

EXIT CODE: 0
```

Finally, an example of why this tool was made - to override DLL references in a templated `App.config` file so the developer can still debug its application from *Visual Studio*:

```xml
<Target Name="AfterBuild" Condition="'$(BuildingInsideVisualStudio)' == 'true'">
  <PropertyGroup>
    <FullOutputPath>$([System.IO.Path]::GetFullPath($([System.IO.Path]::Combine('$(MSBuildProjectDirectory)', '$(OutputPath)'))))</FullOutputPath>
    <AppConfigPath>$([System.IO.Path]::Combine('$(MSBuildProjectDirectory)', 'App.config'))</AppConfigPath>
    <ExeConfigPath>$([System.IO.Path]::Combine('$(FullOutputPath)', '$(AssemblyName).exe.config'))</ExeConfigPath>
  </PropertyGroup>
  <Exec Command="ftreplace.exe -i '$(AppConfigPath)' -o '$(ExeConfigPath)' -f SHARED_LIBS_PATH -r '$(FullOutputPath)'"/>
</Target>
```

**Notes:**
- `ftreplace.exe` is in the `PATH`.
- Referenced assemblies have `CopyLocal` set to `true`.

## License
This project is licensed under the [MIT license](LICENSE) so feel free to use it and/or contribute.

If the tool is to be used in a strong-name environment feel free to sign it with the appropriate `.snk` file.

## TODOs
- [ ] Add the `--ignore-case` switch.
- [ ] Accept a folder to be specified as destination in which case the source file name and extension should be appended.
- [ ] Detect `-i == -o` and prevent processing?
- [x] Make it a [NuGet](https://www.nuget.org/) package and publish it.
