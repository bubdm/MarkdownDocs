using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IParameterContext : IParameterMetadata
    {
        void ParameterType(ITypeContext type);
    }
}