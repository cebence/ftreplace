# File Text Replace (ftreplace) utility

[![Build status](https://ci.appveyor.com/api/projects/status/0reuf6rvv57crl25?svg=true)](https://ci.appveyor.com/project/cebence/ftreplace)
[![NuGet version](https://img.shields.io/nuget/v/FileTextReplace.svg)](https://www.nuget.org/packages/FileTextReplace/)

A simple command-line utility that came out of necessity to rewrite C# project's `App.config` and redirect `codebase` references into project's `Debug` folder.

The tool can copy a source (template) file to the destination location, and/or replace all occurrences of `find` string with the `replace` string in the destination file.

The tool will exit with a `0` exit code if everything was OK. Otherwise, if any of the arguments is missing and/or invalid, files are inaccessible etc. exit code will be `1`.

## Command-line arguments

- `-i <filename>` - Source filename, optional - if omitted equals destination filename.
- `-o <filename>` - Destination filename, required.
- `-f <string>` - Text to find (plain text, no regular expressions), optional.
- `-r <string>` - Replacement text (plain text, no regular expressions), optional.
- `--ignore-case` - Performs case-insensitive string comparisons.
- `--help` - Displays how the tool is supposed to be used.
- `--debug` - Displays all values (filenames and strings).

If you omit both `-f` and `-r` the tool will just copy the text from one file to another.
If you omit only one of them the tool will exit with an error.

> **Note:** If you are calling the tool from *Visual Studio* or *MSBuild*, and your find/replace text contains placeholders surrounded with `%`, e.g. `ftreplace -f %SOMETHING%`, **do not forget** to double the `%` like this `ftreplace -f %%SOMETHING%%`.
> It seems that MSBuild's `Exec` task is trying to resolve anything that resembles an environment variable, and you (actually the tool :smile:) would get an empty string.

<a name="backslash-issue"></a>**IMPORTANT:** I do need to warn you about a nasty issue in this tool :cry: that is not under my control.
If you pass a path ending with a backslash, e.g. `ftreplace -f "C:\Program Files\MyApp\"`, the tool will actually get `C:\Program Files\MyApp"` and will not be able to find it. Or worse, `ftreplace -r "C:\Program Files\MyApp\"` will apply an invalid path.

> Due to a *feature* :smirk: in the way [Windows' CommandLineToArgvW function (`shell32.dll`)](https://msdn.microsoft.com/en-us/library/windows/desktop/bb776391%28v=vs.85%29.aspx) parses command-line arguments almost all applications - confirmed for .NET and Java - are prone to this issue.
More details on the issue can be found in these articles:
- [Commandline args ending in \" are subject to CommandLineToArgvW whackiness](http://weblogs.asp.net/jongalloway//_5B002E00_NET-Gotcha_5D00_-Commandline-args-ending-in-_5C002200_-are-subject-to-CommandLineToArgvW-whackiness)
- [Backslash and quote in command line arguments](http://stackoverflow.com/questions/9287812/backslash-and-quote-in-command-line-arguments)
- [Escape command line arguments in c#](http://stackoverflow.com/questions/5510343/escape-command-line-arguments-in-c-sharp)
- [And this great article](http://www.pseale.com/blog/IHateYouOutDirParameter.aspx)

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

This command will replace "user", "USER", "UsEr" etc. into "User", and produce the following output:

```
> ftreplace -o users.txt -f user -r User --ignore-case --debug

IN: (same as OUT)
OUT: users.txt
FIND (case-insensitive): user
REPLACE: User

EXIT CODE: 0
```

Finally, an example of why this tool was made - to override DLL references in a templated `App.config` file so the developer can still debug its application from *Visual Studio*:

```xml
<Target Name="AfterBuild" Condition="'$(BuildingInsideVisualStudio)' == 'true'">
  <PropertyGroup>
    <FullOutputPath>$([System.IO.Path]::GetFullPath($([System.IO.Path]::Combine('$(MSBuildProjectDirectory)', '$(OutputPath.TrimEnd('\'))'))))</FullOutputPath>
    <AppConfigPath>$([System.IO.Path]::Combine('$(MSBuildProjectDirectory)', 'App.config'))</AppConfigPath>
    <ExeConfigPath>$([System.IO.Path]::Combine('$(FullOutputPath)', '$(AssemblyName).exe.config'))</ExeConfigPath>
  </PropertyGroup>
  <Exec Command='ftreplace.exe -i "$(AppConfigPath)" -o "$(ExeConfigPath)" -f SHARED_LIBS_PATH -r "$(FullOutputPath)"'/>
</Target>
```

**Notes:**
- `ftreplace.exe` is in the `PATH`.
- Referenced assemblies have `CopyLocal` set to `true`.
- Notice the `OutputPath.TrimEnd('\')` to solve the [backslash issue mentioned above](#backslash-issue).

## License
This project is licensed under the [MIT license](LICENSE) so feel free to use it and/or contribute.

If the tool is to be used in a strong-name environment feel free to sign it with the appropriate `.snk` file.

## TODOs
- [x] Add the `--ignore-case` switch.
- [ ] Accept a folder to be specified as destination in which case the source file name and extension should be appended.
- [ ] Detect `-i == -o` and prevent processing?
- [x] Make it a [NuGet](https://www.nuget.org/) package and publish it.
