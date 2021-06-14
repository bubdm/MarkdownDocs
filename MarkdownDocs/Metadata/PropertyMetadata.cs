namespace MarkdownDocs.Metadata
{
    public enum AccessModifier
    {
        Public,
        Protected
    }

    public class PropertyMetadata
    {
        public PropertyMetadata(int id, TypeMetadata owner)
        {
            Id = id;
            Owner = owner;
        }

        public int Id { get; }
        public TypeMetadata Owner { get; }
        public bool IsVirtual { get; set; }
    }
}