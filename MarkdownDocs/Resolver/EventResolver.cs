using MarkdownDocs.Context;
using MarkdownDocs.Metadata;
using System.Reflection;

namespace MarkdownDocs.Resolver
{
    public interface IEventResolver
    {
        IEventContext Resolve(EventInfo ev);
    }

    public class EventResolver : IEventResolver
    {
        private readonly ITypeContext _typeContext;
        private readonly ITypeResolver _typeResolver;

        public EventResolver(ITypeContext typeContext, ITypeResolver typeResolver)
        {
            _typeContext = typeContext;
            _typeResolver = typeResolver;
        }

        public IEventContext Resolve(EventInfo ev)
        {
            IEventContext context = _typeContext.Event(ev.GetHashCode());

            return context;
        }
    }
}
