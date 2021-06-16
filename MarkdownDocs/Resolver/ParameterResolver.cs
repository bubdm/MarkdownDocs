using MarkdownDocs.Metadata;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public interface IParameterResolver
    {
        IParameterMetadata Resolve(ParameterInfo parameter);
    }

    public class ParameterResolver : IParameterResolver
    {
        private readonly IMethodContext _context;
        private readonly ITypeResolver _typeResolver;

        public ParameterResolver(IMethodContext context, ITypeResolver typeResolver)
        {
            _context = context;
            _typeResolver = typeResolver;
        }

        public IParameterMetadata Resolve(ParameterInfo parameter)
        {
            IParameterMetadata meta = _context.Parameter(parameter.GetHashCode());
            meta.Name = parameter.Name;
            meta.Type = _typeResolver.Resolve(parameter.ParameterType);

            return meta;
        }
    }
}
