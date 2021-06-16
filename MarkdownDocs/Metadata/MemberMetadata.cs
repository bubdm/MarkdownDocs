namespace MarkdownDocs.Metadata
{
    public abstract class MemberMetadata : IMemberMetadata
    {
        public MemberMetadata(int id, ITypeContext cpmtext)
        {
            Id = id;
            Context = cpmtext;
        }

        public int Id { get; }
        public ITypeContext Context { get; }
        public string Name { get; set; } = default!;
        public AccessModifier AccessModifier { get; set; }
    }
}