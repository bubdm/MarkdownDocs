namespace MarkdownDocs.Metadata
{
    public interface IParameterMetadata
    {
        int Id { get; }
        ITypeMetadata Type { get; }
        string? Name { get; set; }
    }
}