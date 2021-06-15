using MarkdownDocs.Metadata;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public static class EventResolver
    {
        public static EventMetadata Event(this IAssemblyBuilder builder, in TypeMetadata typeMeta, in EventInfo ev)
        {
            EventMetadata eventMeta = typeMeta.Event(ev.GetHashCode());
            return eventMeta;
        }
    }
}
