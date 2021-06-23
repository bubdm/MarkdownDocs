using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IEventContext : IDocContext
    {
        AccessModifier? RemoveMethodModifier { get; set; }
        AccessModifier? AddMethodModifier { get; set; }
        MethodModifier EventModifier { get; set; }

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
        public AccessModifier? RemoveMethodModifier { get; set; }
        public AccessModifier? AddMethodModifier { get; set; }
        public MethodModifier EventModifier { get; set; }

        public override AccessModifier AccessModifier => AddMethodModifier == AccessModifier.Public || RemoveMethodModifier == AccessModifier.Public
                ? AccessModifier.Public
                : AddMethodModifier ?? RemoveMethodModifier ?? AccessModifier.Unknown;

        public void EventType(ITypeContext type)
        {
            Type = type.GetMetadata();
            type.Reference(Context);
        }

        public IEventMetadata GetMetadata() => this;
    }
}
