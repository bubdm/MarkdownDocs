using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IParameterContext
    {
        void ParameterType(ITypeContext type);
        IParameterMetadata GetMetadata();
    }
}