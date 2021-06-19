namespace MarkdownDocs.Metadata
{
    public interface IMemberMetadata
    {
        int Id { get; }
        string Name { get; }
        // Type containing the field, property or event
        ITypeMetadata Owner { get; }
        AccessModifier AccessModifier { get; }
    }
}