using MarkdownDocs.Metadata;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public static class MethodResolver
    {
        public static MethodMetadata Method(this IAssemblyBuilder builder, in TypeMetadata typeMeta, in MethodInfo method)
        {
            MethodMetadata methodMeta = typeMeta.Method(method.GetHashCode());
            return methodMeta;
        }
    }
}
