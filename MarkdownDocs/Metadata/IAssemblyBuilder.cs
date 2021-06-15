namespace MarkdownDocs.Metadata
{
    public interface IAssemblyBuilder
    {
        TypeMetadata Type(int id);
        IAssemblyBuilder WithName(string? assemblyName);
        IAssemblyMetadata Build();
    }
}