namespace MarkdownDocs.Metadata
{
    public enum AccessModifier
    {
        Unknown,
        Public,
        Protected
    }

    public class PropertyMetadata : MemberMetadata
    {
        public PropertyMetadata(int id, TypeContext owner) : base(id, owner)
        {
        }

        public bool IsVirtual { get; set; }
    }
}