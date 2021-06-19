namespace MarkdownDocs.Metadata
{
    public interface IEventMetadata : IMemberMetadata
    {
        ITypeMetadata Type { get; }
    }
}