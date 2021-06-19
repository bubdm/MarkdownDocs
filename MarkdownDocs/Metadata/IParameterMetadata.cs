namespace MarkdownDocs.Metadata
{
    public interface IParameterMetadata
    {
        int Id { get; }
        string? Name { get; set; }
        ITypeMetadata Type { get; }
        // Type containing the method or constructor
        ITypeMetadata Owner { get; }
    }
}