using MarkdownDocs.Metadata;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public static class PropertyResolver
    {
        public static PropertyMetadata Property(this IAssemblyBuilder builder, in TypeMetadata typeMeta, in PropertyInfo property)
        {
            PropertyMetadata propMeta = typeMeta.Property(property.GetHashCode());
            return propMeta;
        }
    }
}
