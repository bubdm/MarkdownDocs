using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface ITypeContext
    {
        ConstructorMetadata Constructor(int id);
        EventMetadata Event(int id);
        FieldMetadata Field(int id);
        IMethodContext Method(int id);
        PropertyContext Property(int id);
        void Inherit(ITypeContext type);
        void Derive(ITypeContext type);
        void Implement(ITypeContext type);
        void Reference(ITypeContext type);
        ITypeMetadata GetMetadata();
    }
}