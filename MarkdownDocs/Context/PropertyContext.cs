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

        public IPropertyMetadata GetMetadata() => this;
    }
}