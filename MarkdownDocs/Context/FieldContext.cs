using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IFieldContext
    {
        FieldModifier FieldModifier { get; set; }
        IFieldMetadata GetMetadata();
    }

    public class FieldContext : MemberMetadata, IFieldMetadata, IFieldContext
    {
        public FieldContext(int id, ITypeContext owner) : base(id, owner.GetMetadata())
        {
        }

        public FieldModifier FieldModifier { get; set; }

        public IFieldMetadata GetMetadata() => this;
    }
}