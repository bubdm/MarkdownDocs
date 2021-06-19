using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IAssemblyContext
    {
        ITypeContext Type(int id);
        IAssemblyContext WithName(string? assemblyName);
        IAssemblyMetadata GetMetadata();
    }
}