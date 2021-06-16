namespace MarkdownDocs.Metadata
{
    public interface IMemberMetadata
    {
        int Id { get; }
        ITypeContext Context { get; }
        string Name { get; set; }
        AccessModifier AccessModifier { get; set; }
    }
}