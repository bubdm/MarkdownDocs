namespace MarkdownDocs.Metadata
{
    public interface IPropertyMetadata : IMemberMetadata
    {
        string? RawValue { get; }
        ITypeMetadata Type { get; }
        MethodModifier PropertyModifier { get; }
        AccessModifier? GetMethodModifier { get; }
        AccessModifier? SetMethodModifier { get; }
    }
}
