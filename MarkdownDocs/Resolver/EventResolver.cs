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

            MethodInfo? addMethod = ev.AddMethod;
            MethodInfo? removeMethod = ev.RemoveMethod;

            context.Name = ev.Name;
            context.AddMethodModifier = addMethod?.GetAccessModifier();
            context.RemoveMethodModifier = removeMethod?.GetAccessModifier();

            MethodModifier? addModifier = addMethod?.GetMethodModifier();
            MethodModifier? removeModifier = removeMethod?.GetMethodModifier();

            context.EventModifier = addModifier ?? removeModifier ?? MethodModifier.None;

            if (ev.EventHandlerType != null)
            {
                ITypeContext evType = _typeResolver.Resolve(ev.EventHandlerType);
                context.EventType(evType);
            }

            return context;
        }
    }
}
