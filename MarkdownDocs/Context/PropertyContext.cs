using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IPropertyContext : IDocContext
    {
        string? RawValue { get; set; }
        MethodModifier PropertyModifier { get; set; }
        AccessModifier? GetMethodModifier { get; set; }
        AccessModifier? SetMethodModifier { get; set; }

        void PropertyType(ITypeContext type);
        IPropertyMetadata GetMetadata();
    }

    public class PropertyContext : MemberMetadata, IPropertyMetadata, IPropertyContext
    {
        public PropertyContext(int id, ITypeContext context) : base(id, context.GetMetadata())
        {
            Context = context;
        }

        public ITypeContext Context { get; }
        public string? RawValue { get; set; }
        public ITypeMetadata Type { get; private set; } = default!;
        public MethodModifier PropertyModifier { get; set; }
        public AccessModifier? GetMethodModifier { get; set; }
        public AccessModifier? SetMethodModifier { get; set; }

        public override AccessModifier AccessModifier => GetMethodModifier == AccessModifier.Public || SetMethodModifier == AccessModifier.Public
                ? AccessModifier.Public
                : GetMethodModifier ?? SetMethodModifier ?? AccessModifier.Unknown;

        public IPropertyMetadata GetMetadata() => this;

        public void PropertyType(ITypeContext type)
        {
            Type = type.GetMetadata();
            type.Reference(Context);
        }
    }
}