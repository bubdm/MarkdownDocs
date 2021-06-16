namespace MarkdownDocs.Metadata
{
    public interface ITypeContext : ITypeMetadata
    {
        ConstructorMetadata Constructor(int id);
        EventMetadata Event(int id);
        FieldMetadata Field(int id);
        IMethodContext Method(int id);
        PropertyMetadata Property(int id);
        void Inherit(ITypeContext type);
        void Derive(ITypeContext type);
        void Implement(ITypeContext type);
        void Reference(ITypeContext type);
    }
}