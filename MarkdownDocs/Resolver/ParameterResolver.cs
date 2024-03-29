﻿using MarkdownDocs.Context;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public interface IParameterResolver
    {
        IParameterContext Resolve(ParameterInfo parameter);
    }

    public class ParameterResolver : IParameterResolver
    {
        private readonly IMethodBaseContext _context;
        private readonly ITypeResolver _typeResolver;

        public ParameterResolver(IMethodBaseContext context, ITypeResolver typeResolver)
        {
            _context = context;
            _typeResolver = typeResolver;
        }

        public IParameterContext Resolve(ParameterInfo parameter)
        {
            IParameterContext context = _context.Parameter(parameter.GetHashCode());
            context.Name = parameter.Name;

            ITypeContext type = _typeResolver.Resolve(parameter.ParameterType);
            context.ParameterType(type);

            context.RawValue = parameter.RawDefaultValue.ToLiteralString();

            return context;
        }
    }
}
