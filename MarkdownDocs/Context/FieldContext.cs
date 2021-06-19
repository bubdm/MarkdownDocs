using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IFieldContext
    {
        string Name { get; set; }
        string? RawValue { get; set; }
        FieldModifier FieldModifier { get; set; }
        AccessModifier AccessModifier { get; set; }
        void FieldType(ITypeContext type);
        IFieldMetadata GetMetadata();
    }

    public class FieldContext : MemberMetadata, IFieldMetadata, IFieldContext
    {
        public FieldContext(int id, ITypeContext context) : base(id, context.GetMetadata())
        {
            Context = context;
        }

        public ITypeContext Context { get; }
        public string? RawValue { get; set; }
        public FieldModifier FieldModifier { get; set; }
        public ITypeMetadata Type { get; private set; } = default!;

        public IFieldMetadata GetMetadata() => this;

        public void FieldType(ITypeContext type)
        {
            Type = type.GetMetadata();
            type.Reference(Context);
        }
    }
}