using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownDocs.CLI
{
    public class OptionsBuilderResult
    {
        public static string FileIsEmpty { get; } = "Input file path is empty.";
        public static string InvalidFileExtension { get; } = "File '{0}' must have one of the following extensions: {1}.";
        public static string MissingOutputPath { get; } = "Missing output path.";
        public static string FileDoesNotExist { get; } = "File '{0}' does not exist.";
        public static string ParallelNotAllowedWithCompact { get; } = "Compact mode does not work with parallel writes.";

        public DocsOptions? Options { get; set; }

        private readonly List<string> _errors = new List<string>();
        public IReadOnlyCollection<string> Errors => _errors;

        public bool IsValid => _errors.Count == 0;

        public void AddError(string error, params string[] args)
        {
            var formatted = string.Format(error, args);
            _errors.Add(formatted);
        }
    }

    public class OptionsBuilder
    {
        private readonly string[] _allowedExtensions = new string[] { ".dll", ".exe" };
        private const string _markdownExtension = ".md";

        public OptionsBuilder(string[] args) => Args = args;

        public string[] Args { get; }

        public OptionsBuilderResult Build()
        {
            var result = new OptionsBuilderResult();

            string fileName = Args[0];
            if (string.IsNullOrWhiteSpace(fileName) || fileName.StartsWith("-"))
            {
                result.AddError(OptionsBuilderResult.FileIsEmpty);
            }
            else if (!_allowedExtensions.Contains(Path.GetExtension(fileName)))
            {
                result.AddError(OptionsBuilderResult.InvalidFileExtension, fileName, string.Join(", ", _allowedExtensions));
            }
            else if (!File.Exists(fileName))
            {
                result.AddError(OptionsBuilderResult.FileDoesNotExist, fileName);
            }

            CommandOption? output = ParseCommand("-o") ?? ParseCommand("--output");
            if (output == null || string.IsNullOrWhiteSpace(output.Argument))
            {
                result.AddError(OptionsBuilderResult.MissingOutputPath);
            }

            if (result.IsValid)
            {
                CommandOption? noxml = ParseCommand("-nx") ?? ParseCommand("--noxml");
                CommandOption? compact = ParseCommand("-c") ?? ParseCommand("--compact");
                CommandOption? parallelWrites = ParseCommand("-pw") ?? ParseCommand("--parallel-writes");
                bool isCompact = compact != null;
                bool pw = parallelWrites != null;
                bool useXml = noxml == null;

                if (pw && isCompact)
                {
                    result.AddError(OptionsBuilderResult.ParallelNotAllowedWithCompact);
                }

                if (result.IsValid)
                {
                    string inputPath = Path.GetFullPath(fileName);
                    string outputPath = Path.GetFullPath(output!.Argument!) ?? Directory.GetCurrentDirectory();

                    if (isCompact && !Path.HasExtension(outputPath))
                    {
                        outputPath = Path.Join(outputPath, Path.ChangeExtension(Path.GetFileName(inputPath), _markdownExtension));
                    }

                    result.Options = new DocsOptions(inputPath, outputPath, isCompact, useXml, pw);
                }
            }

            return result;
        }

        private CommandOption? ParseCommand(string command)
        {
            for (var i = 0; i < Args.Length; i++)
            {
                if (Args[i] == command)
                {
                    var cmd = new CommandOption(command);

                    if (i + 1 < Args.Length)
                    {
                        var arg = Args[i + 1].Trim();
                        if (!arg.StartsWith("-"))
                        {
                            cmd.Argument = arg;
                        }
                    }

                    return cmd;
                }
            }

            return null;
        }

        private class CommandOption
        {
            public string Name { get; }
            public string? Argument { get; set; }

            public CommandOption(string name) => Name = name;
        }
    }
}
