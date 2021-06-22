using MarkdownDocs.Context;
using MarkdownDocs.Metadata;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public interface IPropertyResolver
    {
        IPropertyContext Resolve(PropertyInfo property);
    }

    public class PropertyResolver : IPropertyResolver
    {
        private readonly ITypeContext _typeContext;
        private readonly ITypeResolver _typeResolver;

        public PropertyResolver(ITypeContext typeContext, ITypeResolver typeResolver)
        {
            _typeContext = typeContext;
            _typeResolver = typeResolver;
        }

        public IPropertyContext Resolve(PropertyInfo property)
        {
            IPropertyContext context = _typeContext.Property(property.GetHashCode());

            MethodInfo? getMethod = property.GetMethod;
            MethodInfo? setMethod = property.SetMethod;

            context.Name = property.Name;
            context.GetMethodModifier = getMethod?.GetAccessModifier();
            context.SetMethodModifier = setMethod?.GetAccessModifier();

            if (property.Attributes.HasFlag(PropertyAttributes.HasDefault))
            {
                object? rawValue = property.GetRawConstantValue();
                context.RawValue = rawValue.ToLiteralString();
            }

            MethodModifier? getModifier = getMethod?.GetMethodModifier();
            MethodModifier? setModifier = setMethod?.GetMethodModifier();

            context.PropertyModifier = getModifier ?? setModifier ?? MethodModifier.None;

            ITypeContext propertyType = _typeResolver.Resolve(property.PropertyType);
            context.PropertyType(propertyType);
            return context;
        }
    }
}
