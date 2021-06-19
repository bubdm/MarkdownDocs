using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public class ParameterContext : IParameterContext
    {
        private readonly ITypeContext _owner;

        public ParameterContext(ITypeContext owner, int id)
        {
            _owner = owner;
            Id = id;
        }

        public int Id { get; }
        public ITypeMetadata Type { get; private set; } = default!;
        public string? Name { get; set; }

        public void ParameterType(ITypeContext type)
        {
            Type = type;
            type.Reference(_owner);
        }
    }
}