namespace MarkdownDocs.CLI
{
    public class Options : DocsOptions
    {
        public string InputPath { get; }
        public string OutputPath { get; }

        public Options(string inputPath, string outputPath, bool isCompact, bool useXML)
            : base(isCompact, useXML)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
        }
    }
}
