using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IMethodContext : IMethodMetadata
    {
        IParameterContext Parameter(int id);
        void Return(ITypeContext type);
    }
}