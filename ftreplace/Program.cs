/// Author: https://github.com/cebence
/// License: MIT

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace FileTextReplace {
  /// <summary>
  /// FileTextReplace application copies the specified input file's (text)
  /// content to the specified output file while replacing all occurences
  /// of "find" string with the "replace" string.
  /// </summary>
  public class Program {
    #region Command-line options
    private const String ARG_DEBUG = "--debug";
    private const String ARG_HELP = "--help";
    private const String ARG_IGNORE_CASE = "--ignore-case";
    private const String ARG_INPUT = "-i";
    private const String ARG_OUTPUT = "-o";
    private const String ARG_FIND = "-f";
    private const String ARG_REPLACE = "-r";
    #endregion

    #region Exit codes
    public const int EXIT_OK = 0;
    public const int EXIT_ERROR = 1;
    #endregion

    private Boolean debugMode;
    private Boolean justTheHelp;
    private Boolean ignoreCase;
    private String inputFilename;
    private String outputFilename;
    private String findWhat;
    private String replaceWith;
    private Boolean justCopyText;
    private String finalText = String.Empty;

    public static void Main(String[] args) {
      Program p = new Program(args);

      if (p.justTheHelp) {
        ShowUsage();
        return;
      }

      int exitCode = EXIT_OK;

      try {
        p.Execute();
      }
      catch (Exception e) {
        exitCode = EXIT_ERROR;

        Console.WriteLine("ERROR: {0}", e.Message);
        if (e.InnerException != null) {
          Console.WriteLine("  Caused by: {0}", e.InnerException.Message);
        }
      }

      DebugDump(p, exitCode);

      Environment.Exit(exitCode);
    }

    public static void DebugDump(Program p, int exitCode) {
      if (p.debugMode) {
        Console.WriteLine("IN: {0}", p.inputFilename != null
            ? p.inputFilename
            : "(same as OUT)");
        Console.WriteLine("OUT: {0}", p.outputFilename);
        if (!p.justCopyText) {
          Console.WriteLine("FIND{0}: {1}", p.ignoreCase ? " (case-insensitive)" : "", p.findWhat);
          Console.WriteLine("REPLACE: {0}", p.replaceWith);
          Console.WriteLine("TEXT: {0}", p.finalText);
        }
        else {
          Console.WriteLine("FIND: (nothing, just copy)");
          Console.WriteLine("REPLACE: (nothing, just copy)");
          Console.WriteLine("TEXT: ('type {0}')", p.outputFilename);
        }

        Console.WriteLine();
        Console.WriteLine("EXIT CODE: {0}", exitCode);
      }
    }

    public Program(String[] args) {
      ParseArguments(args);
    }

    private void ParseArguments(String[] arguments) {
      List<String> args = new List<String>(arguments);

      debugMode = GetSwitch(args, ARG_DEBUG);
      justTheHelp = GetSwitch(args, ARG_HELP);
      ignoreCase = GetSwitch(args, ARG_IGNORE_CASE);

      inputFilename = GetArgumentValue(args, ARG_INPUT, null);
      outputFilename = GetArgumentValue(args, ARG_OUTPUT, null);
      findWhat = GetArgumentValue(args, ARG_FIND, String.Empty);
      replaceWith = GetArgumentValue(args, ARG_REPLACE, String.Empty);
    }

    public void ValidateArguments() {
      // Destination filename is required.
      if (outputFilename == null) {
        throw new Exception("Destination filename was not specified.");
      }

      // Can't specify just one of the find/replace parameters.
      if (!String.Empty.Equals(findWhat) || !String.Empty.Equals(replaceWith)) {
        if (String.Empty.Equals(findWhat)) {
          throw new Exception("Text to find was not specified.");
        }
        if (String.Empty.Equals(replaceWith)) {
          throw new Exception("Replacement text was not specified.");
        }
      }
    }

    private static void ShowUsage() {
      Console.WriteLine("FileTextReplace - replaces text from input to output text file.");
      Console.WriteLine();
      Console.WriteLine("Usage: ftreplace [options]");
      Console.WriteLine();
      Console.WriteLine("  -i <FILENAME>   File to process, optional.");
      Console.WriteLine("  -o <FILENAME>   File to produce (output file).");
      Console.WriteLine("  -f <STRING>     Text to find, optional together with -r.");
      Console.WriteLine("  -r <STRING>     Replacement text, optional together with -f.");
      Console.WriteLine("  --ignore-case   Performs case-insensitive string comparisons.");
      Console.WriteLine("  --help          Displays how the tool is supposed to be used.");
      Console.WriteLine("  --debug         Displays all values (filenames and strings).");
      Console.WriteLine();
      Console.WriteLine("Examples:");
      Console.WriteLine("- Replace any frogs to oranges in the 'oranges.txt':");
      Console.WriteLine("  ftreplace -o oranges.txt -f frog -r orange");
      Console.WriteLine();
      Console.WriteLine("- Set the user to 'Guest' in the 'app.config':");
      Console.WriteLine("  ftreplace -i config.template -o app.config -f {{USER}} -r Guest");
      Console.WriteLine();
      Console.WriteLine("- Just copy and rename the file:");
      Console.WriteLine("  ftreplace -i config.template -o app.config");
      Console.WriteLine();
      Console.WriteLine("- Replace 'user', 'USER', 'UsEr' etc. into 'User':");
      Console.WriteLine("  ftreplace -o a.txt -f user -r User --ignore-case");
    }

    public void Execute() {
      ValidateArguments();

      String text = LoadTextFromFile(inputFilename != null
          ? inputFilename
          : outputFilename);

      // Should we replace text or just copy it?
      justCopyText = String.Empty.Equals(findWhat)
          && String.Empty.Equals(replaceWith);

      if (!justCopyText) {
        text = ReplaceText(text, findWhat, replaceWith, ignoreCase);
        finalText = text;
      }

      SaveTextToFile(text, outputFilename);
    }

    public static String LoadTextFromFile(String filename) {
      try {
        using (StreamReader reader = new StreamReader(filename)) {
          return reader.ReadToEnd();
        }
      }
      catch (Exception e) {
        throw new Exception(
            String.Format("Could not read from '{0}'.", filename),
            e);
      }
    }

    public static void SaveTextToFile(String text, String filename) {
      try {
        using (StreamWriter writer = new StreamWriter(filename)) {
          writer.Write(text);
        }
      }
      catch (Exception e) {
        throw new Exception(
            String.Format("Could not write to '{0}'.", filename),
            e);
      }
    }

    public static String ReplaceText(String text, String find, String replace, Boolean ignoreCase) {
      if (!ignoreCase) {
        return text.Replace(find, replace);
      }

      return Regex.Replace(text, find, replace, RegexOptions.IgnoreCase);
    }

    #region Utility methods

    /// <summary>
    /// Tries to find the named on/off argument in the list.
    /// </summary>
    /// <remarks>
    /// Assumption is that argument's presence means ON, and its absence OFF.
    /// </remarks>
    public static Boolean GetSwitch(List<String> args, String name) {
      return args.Contains(name);
    }

    /// <summary>
    /// Returns the named argument's value in the list (if it's found),
    /// or the default value.
    /// </summary>
    /// <remarks>
    /// Assumption is that argument value comes immediately after its name,
    /// i.e. <c>{ "name1", "value1", "name2", "value2" }</c>.
    /// </remarks>
    public static String GetArgumentValue(List<String> args, String name, String defaultValue) {
      int index = args.IndexOf(name);
      if (index >= 0 && args.Count > index + 1) {
        return args[index + 1];
      }

      return defaultValue;
    }

    #endregion
  }
}
