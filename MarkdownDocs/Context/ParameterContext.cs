using MarkdownDocs.Metadata;

namespace MarkdownDocs.Context
{
    public interface IParameterContext : IDocContext
    {
        string? RawValue { get; set; }
        void ParameterType(ITypeContext type);
        IParameterMetadata GetMetadata();
    }

    public class ParameterContext : IParameterMetadata, IParameterContext
    {
        private readonly ITypeContext _owner;

        public ParameterContext(int id, ITypeContext owner)
        {
            Id = id;
            _owner = owner;
        }

        public int Id { get; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? RawValue { get; set; }
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