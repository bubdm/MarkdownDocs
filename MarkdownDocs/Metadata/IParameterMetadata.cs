namespace MarkdownDocs.Metadata
{
    public interface IParameterMetadata : IDocMetadata
    {
        string? RawValue { get; }
        ITypeMetadata Type { get; }
        // Type containing the method or constructor
        ITypeMetadata Owner { get; }
    }
}