using MarkdownDocs.Context;
using MarkdownDocs.Metadata;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public static class FieldResolver
    {
        public static FieldMetadata Field(this IAssemblyContext builder, in ITypeContext typeMeta, in FieldInfo field)
        {
            FieldMetadata fieldMeta = typeMeta.Field(field.GetHashCode());
            return fieldMeta;
        }
    }
}
