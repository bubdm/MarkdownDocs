using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IEventContext
    {
        void EventType(ITypeContext type);
        IEventMetadata GetMetadata();
    }

    public class EventContext : MemberMetadata, IEventMetadata, IEventContext
    {
        public EventContext(int id, ITypeContext context) : base(id, context.GetMetadata())
        {
            Context = context;
        }

        public ITypeContext Context { get; }
        public ITypeMetadata Type { get; private set; } = default!;

        public void EventType(ITypeContext type)
        {
            Type = type.GetMetadata();
            type.Reference(Context);
        }

        public IEventMetadata GetMetadata() => this;
    }
}
