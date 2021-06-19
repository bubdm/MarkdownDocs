using MarkdownDocs.Context;
using MarkdownDocs.Metadata;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public interface IFieldResolver
    {
        IFieldContext Resolve(FieldInfo field);
    }

    public class FieldResolver : IFieldResolver
    {
        private readonly ITypeContext _typeContext;
        private readonly ITypeResolver _typeResolver;

        public FieldResolver(ITypeContext typeContext, ITypeResolver typeResolver)
        {
            _typeContext = typeContext;
            _typeResolver = typeResolver;
        }

        public IFieldContext Resolve(FieldInfo field)
        {
            IFieldContext context = _typeContext.Field(field.GetHashCode());

            context.Name = field.Name;

            if (field.IsPublic)
            {
                context.AccessModifier = AccessModifier.Public;
            }
            else if (field.IsFamily)
            {
                context.AccessModifier = AccessModifier.Protected;
            }

            if (field.IsLiteral)
            {
                context.FieldModifier = FieldModifier.Const;

                object? rawValue = field.GetRawConstantValue();
                context.RawValue = rawValue.ToLiteralString();
            }
            else if (field.IsStatic)
            {
                context.FieldModifier = FieldModifier.Static;
            }
            else if (field.IsInitOnly)
            {
                context.FieldModifier = FieldModifier.Readonly;
            }

            ITypeContext fieldType = _typeResolver.Resolve(field.FieldType);
            context.FieldType(fieldType);
            return context;
        }
    }
}
