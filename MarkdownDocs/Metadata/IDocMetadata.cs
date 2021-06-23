namespace MarkdownDocs.Metadata
{
    public interface IDocMetadata
    {
        int Id { get; }
        string Name { get; }
        string? Description { get; }
    }
}