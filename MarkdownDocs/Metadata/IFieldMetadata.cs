namespace MarkdownDocs.Metadata
{
    public interface IFieldMetadata : IMemberMetadata
    {
        FieldModifier FieldModifier { get; }
        ITypeMetadata Type { get; }
        string? RawValue { get; }
    }
}
