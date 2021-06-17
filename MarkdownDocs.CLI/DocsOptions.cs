namespace MarkdownDocs.CLI
{
    public class DocsOptions : IDocsOptions
    {
        public bool IsCompact { get; }
        public bool UseXML { get; }
        public bool ParallelWrites { get; }
        public bool NoNamespace => false;

        public string BaseUrl => string.Empty;
        public string InputPath { get; }
        public string OutputPath { get; }

        public DocsOptions(string inputPath, string outputPath, bool isCompact, bool useXML, bool parallelWrites)
        {
            InputPath = inputPath;
            OutputPath = outputPath;
            IsCompact = isCompact;
            UseXML = useXML;
            ParallelWrites = parallelWrites;
        }
    }
}
