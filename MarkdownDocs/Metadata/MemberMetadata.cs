namespace MarkdownDocs.Metadata
{
    public abstract class MemberMetadata : IMemberMetadata
    {
        public MemberMetadata(int id, ITypeMetadata owner)
        {
            Id = id;
            Owner = owner;
        }

        public int Id { get; }
        public ITypeMetadata Owner { get; }
        public string Name { get; set; } = default!;
        public AccessModifier AccessModifier { get; set; }
    }
}