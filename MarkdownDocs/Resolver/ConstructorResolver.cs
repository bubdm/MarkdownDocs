using MarkdownDocs.Metadata;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public static class ConstructorResolver
    {
        public static ConstructorMetadata Constructor(this IAssemblyContext builder, in TypeContext typeMeta, in ConstructorInfo ctor)
        {
            ConstructorMetadata ctorMeta = typeMeta.Constructor(ctor.GetHashCode());
            return ctorMeta;
        }
    }
}
