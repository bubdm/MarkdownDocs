using MarkdownDocs.Context;
using MarkdownDocs.Metadata;
using System;
using System.Linq;
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

        public MethodResolver(ITypeContext typeContext, ITypeResolver typeResolver, Func<IMethodContext, ITypeResolver, IParameterResolver> parameterResolverFactory)
        {
            _typeContext = typeContext;
            _parameterResolverFactory = parameterResolverFactory;
            _typeResolver = typeResolver;
        }

        public IMethodContext Resolve(MethodInfo method)
        {
            IMethodContext context = _typeContext.Method(method.GetHashCode());
            IMethodMetadata meta = context.GetMetadata();

            if (method.IsGenericMethod)
            {
                meta.Name = method.Name.Split('`')[0] + "<" + string.Join(", ", method.GetGenericArguments().Select(t => t.ToPrettyName()).ToArray()) + ">";
            }
            else
            {
                meta.Name = method.Name;
            }

            if (method.IsVirtual)
            {
                MethodInfo baseMethod = method.GetBaseDefinition();
                if (baseMethod != method)
                {
                    meta.MethodModifier = MethodModifier.Override;
                }
                else
                {
                    meta.MethodModifier = MethodModifier.Virtual;
                }
            }
            else if(method.IsAbstract)
            {
                meta.MethodModifier = MethodModifier.Abstract;
            }
            else if (method.IsStatic)
            {
                meta.MethodModifier = MethodModifier.Static;
            }

            if (method.IsPublic)
            {
                meta.AccessModifier = AccessModifier.Public;
            }
            else if (method.IsFamily)
            {
                meta.AccessModifier = AccessModifier.Protected;
            }

            IParameterResolver resolver = _parameterResolverFactory(context, _typeResolver);
            foreach (ParameterInfo param in method.GetParameters())
            {
                resolver.Resolve(param);
            }

            ITypeContext returnType = _typeResolver.Resolve(method.ReturnType);
            context.Return(returnType);
            return context;
        }
    }
}
