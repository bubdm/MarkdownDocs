namespace MarkdownDocs.Metadata
{
    public interface IEventMetadata : IMemberMetadata
    {
        MethodModifier EventModifier { get; }
        ITypeMetadata Type { get; }
    }
}