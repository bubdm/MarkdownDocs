using MarkdownDocs.Context;
using MarkdownDocs.Metadata;
using System;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public interface IConstructorResolver
    {
        IConstructorContext Resolve(ConstructorInfo constructor);
    }

    public class ConstructorResolver : IConstructorResolver
    {
        private readonly ITypeContext _typeContext;
        private readonly ITypeResolver _typeResolver;
        private readonly Func<IMethodBaseContext, ITypeResolver, IParameterResolver> _parameterResolverFactory;

        public ConstructorResolver(ITypeContext typeContext, ITypeResolver typeResolver, Func<IMethodBaseContext, ITypeResolver, IParameterResolver> parameterResolverFactory)
        {
            _typeContext = typeContext;
            _typeResolver = typeResolver;
            _parameterResolverFactory = parameterResolverFactory;
        }

        public IConstructorContext Resolve(ConstructorInfo constructor)
        {
            IConstructorContext context = _typeContext.Constructor(constructor.GetHashCode());

            context.Name = constructor.DeclaringType!.Name;
            context.AccessModifier = constructor.IsPublic ? AccessModifier.Public : AccessModifier.Protected;

            IParameterResolver resolver = _parameterResolverFactory(context, _typeResolver);
            foreach (ParameterInfo param in constructor.GetParameters())
            {
                resolver.Resolve(param);
            }

            return context;
        }
    }
}
