using MarkdownDocs.Metadata;
using System;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public interface IMethodResolver
    {
        IMethodContext Resolve(MethodInfo method);
    }

    public class MethodResolver : IMethodResolver
    {
        private readonly ITypeContext _typeContext;
        private readonly Func<IMethodContext, ITypeResolver, IParameterResolver> _parameterResolverFactory;
        private readonly ITypeResolver _typeResolver;

        public MethodResolver(ITypeResolver typeResolver, ITypeContext typeContext, Func<IMethodContext, ITypeResolver,  IParameterResolver> parameterResolverFactory)
        {
            _typeContext = typeContext;
            _parameterResolverFactory = parameterResolverFactory;
            _typeResolver = typeResolver;
        }

        public IMethodContext Resolve(MethodInfo method)
        {
            IMethodContext meta = _typeContext.Method(method.GetHashCode());
            meta.Name = method.Name;
            meta.AccessModifier = method.IsPublic ? AccessModifier.Public : AccessModifier.Protected;

            IParameterResolver resolver = _parameterResolverFactory(meta, _typeResolver);
            foreach (ParameterInfo param in method.GetParameters())
            {
                resolver.Resolve(param);
            }

            meta.ReturnType = _typeResolver.Resolve(method.ReturnType);
            return meta;
        }
    }
}
