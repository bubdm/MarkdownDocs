namespace MarkdownDocs.Metadata
{
    public class MethodMetadata : MemberMetadata
    {
        public MethodMetadata(int id, ITypeContext owner) : base(id, owner)
        {
        }

        public ITypeContext ReturnType { get; set; } = default!;
    }
}