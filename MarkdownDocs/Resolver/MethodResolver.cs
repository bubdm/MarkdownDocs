using MarkdownDocs.Metadata;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public interface IMethodResolver
    {
        MethodMetadata Resolve(MethodInfo method);
    }

    public class MethodResolver : IMethodResolver
    {
        private readonly ITypeContext _typeContext;
        private readonly ITypeResolver _typeResolver;

        public MethodResolver(ITypeResolver typeResolver, ITypeContext typeContext)
        {
            _typeContext = typeContext;
            _typeResolver = typeResolver;
        }

        public MethodMetadata Resolve(MethodInfo method)
        {
            MethodMetadata meta = _typeContext.Method(method.GetHashCode());
            meta.Name = method.Name;

            meta.ReturnType = _typeResolver.Resolve(method.ReturnType);
            return meta;
        }
    }
}
