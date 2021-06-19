namespace MarkdownDocs.Metadata
{
    public interface IMemberMetadata
    {
        int Id { get; }
        ITypeMetadata Owner { get; }
        string Name { get; }
        AccessModifier AccessModifier { get; }
    }
}