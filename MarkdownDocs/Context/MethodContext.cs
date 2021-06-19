using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IMethodContext : IMethodBaseContext
    {
        void Return(ITypeContext type);
        IMethodMetadata GetMetadata();
    }

    public class MethodContext : MethodBaseContext, IMethodMetadata, IMethodContext
    {
        public MethodContext(int id, ITypeContext context) : base(id, context)
        {
        }

        public void Return(ITypeContext type)
        {
            ReturnType = type.GetMetadata();
            type.Reference(Context);
        }

        public IMethodMetadata GetMetadata() => this;

        public ITypeMetadata ReturnType { get; private set; } = default!;
        public MethodModifier MethodModifier { get; set; }
    }
}