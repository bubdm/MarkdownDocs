using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IPropertyContext 
    {
        IPropertyMetadata GetMetadata();
    }

    public class PropertyContext : MemberMetadata, IPropertyMetadata, IPropertyContext
    {
        public PropertyContext(int id, ITypeContext owner) : base(id, owner.GetMetadata())
        {
        }

        public bool IsVirtual { get; set; }

        public IPropertyMetadata GetMetadata() => this;
    }
}