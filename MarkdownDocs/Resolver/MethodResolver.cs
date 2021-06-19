using MarkdownDocs.Context;
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
            context.AccessModifier = method.GetAccessModifier();
            context.MethodModifier = method.GetMethodModifier();

            if (method.IsGenericMethod)
            {
                context.Name = method.Name.Split('`')[0] + "<" + string.Join(", ", method.GetGenericArguments().Select(t => t.ToPrettyName()).ToArray()) + ">";
            }
            else
            {
                context.Name = method.Name;
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
