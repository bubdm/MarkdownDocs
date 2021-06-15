namespace MarkdownDocs.Metadata
{
    public interface ITypeBuilder
    {
        ConstructorMetadata Constructor(int id);
        EventMetadata Event(int id);
        FieldMetadata Field(int id);
        MethodMetadata Method(int id);
        PropertyMetadata Property(int id);
        void Inherit(TypeMetadata type);
        void Implement(TypeMetadata type);
        void Reference(TypeMetadata type);
    }
}