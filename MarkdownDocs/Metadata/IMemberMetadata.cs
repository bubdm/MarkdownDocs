namespace MarkdownDocs.Metadata
{
    public interface IMemberMetadata : IDocMetadata
    {
        // Type containing the field, property or event
        ITypeMetadata Owner { get; }
        AccessModifier AccessModifier { get; }
    }
}