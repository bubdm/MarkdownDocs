namespace MarkdownDocs.Metadata
{
    public interface IMetadataBuilder
    {
        TypeMetadata Type(int id);
        AssemblyMetadata Build();
    }
}
