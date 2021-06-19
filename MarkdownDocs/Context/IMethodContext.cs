using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IMethodContext
    {
        IParameterContext Parameter(int id);
        void Return(ITypeContext type);
        IMethodMetadata GetMetadata();
    }
}