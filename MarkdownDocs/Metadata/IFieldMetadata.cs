namespace MarkdownDocs.Metadata
{
    public interface IFieldMetadata : IMemberMetadata
    {
        FieldModifier FieldModifier { get; }
    }
}
