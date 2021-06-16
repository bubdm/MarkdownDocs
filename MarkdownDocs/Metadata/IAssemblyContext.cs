namespace MarkdownDocs.Metadata
{
    public interface IAssemblyContext : IAssemblyMetadata
    {
        ITypeContext Type(int id);
        IAssemblyContext WithName(string? assemblyName);
        IAssemblyMetadata Build();
    }
}