using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public class PropertyContext : MemberMetadata, IPropertyContext
    {
        public PropertyContext(int id, ITypeContext owner) : base(id, owner)
        {
        }

        public bool IsVirtual { get; set; }
    }
}