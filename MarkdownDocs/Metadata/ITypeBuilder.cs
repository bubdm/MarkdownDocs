namespace MarkdownDocs.Metadata
{
    public interface ITypeBuilder
    {
        ConstructorMetadata Constructor(int id);
        EventMetadata Event(object p);
        FieldMetadata Field(int v);
        TypeMetadata Inherit(TypeMetadata type);
        MethodMetadata Method(int v);
        PropertyMetadata Property(int id);
        void Implement(TypeMetadata type);
        void Reference(TypeMetadata type);
    }
}