namespace MarkdownDocs
{
    public interface IDocsOptions
    {
        string InputPath { get; }
        string OutputPath { get; }
        bool IsCompact { get; }
        bool ParallelWrites { get; }
        bool UseXML { get; }
        bool NoNamespace { get; }
        string BaseUrl { get; }
    }
}