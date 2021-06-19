using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IAssemblyContext : IAssemblyMetadata
    {
        ITypeContext Type(int id);
        IAssemblyContext WithName(string? assemblyName);
        IAssemblyMetadata Build();
    }
}