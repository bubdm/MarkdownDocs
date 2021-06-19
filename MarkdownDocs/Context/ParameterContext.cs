using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IParameterContext
    {
        void ParameterType(ITypeContext type);
        IParameterMetadata GetMetadata();
    }

    public class ParameterContext : IParameterMetadata, IParameterContext
    {
        private readonly ITypeContext _owner;

        public ParameterContext(ITypeContext owner, int id)
        {
            _owner = owner;
            Id = id;
        }

        public int Id { get; }
        public string? Name { get; set; }
        public ITypeMetadata Type { get; private set; } = default!;
        public ITypeMetadata Owner => _owner.GetMetadata();

        public IParameterMetadata GetMetadata() => this;

        public void ParameterType(ITypeContext type)
        {
            Type = type.GetMetadata();
            type.Reference(_owner);
        }
    }
}