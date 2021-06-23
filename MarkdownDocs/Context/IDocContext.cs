namespace MarkdownDocs.Context
{
    public interface IDocContext
    {
        string Name { get; set; }
        string? Description { get; set; }
    }
}