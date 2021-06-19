using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IConstructorContext : IMethodBaseContext
    {
        IConstructorMetadata GetMetadata();
    }

    public class ConstructorContext : MethodBaseContext, IConstructorMetadata, IConstructorContext
    {
        public ConstructorContext(int id, ITypeContext context) : base(id, context)
        {
        }

        public IConstructorMetadata GetMetadata() => this;

    }
}
