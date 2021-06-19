using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface ITypeContext
    {
        string Name { get; set; }
        string? Namespace { get; set; }
        string? Assembly { get; set; }
        string? Company { get; set; }

        AccessModifier AccessModifier { get; set; }
        TypeCategory Category { get; set; }
        TypeModifier Modifier { get; set; }

        IConstructorContext Constructor(int id);
        IFieldContext Field(int id);
        IPropertyContext Property(int id);
        IMethodContext Method(int id);
        IEventContext Event(int id);

        void Inherit(ITypeContext type);
        void Derive(ITypeContext type);
        void Implement(ITypeContext type);
        void Reference(ITypeContext type);

        ITypeMetadata GetMetadata();
    }
}