using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public class ParameterContext : IParameterMetadata, IParameterContext
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

        public IParameterMetadata GetMetadata() => this;

        public void ParameterType(ITypeContext type)
        {
            Type = type.GetMetadata();
            type.Reference(_owner);
        }
    }
}