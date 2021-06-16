namespace MarkdownDocs.Metadata
{
    public interface IParameterMetadata
    {
        int Id { get; }
        ITypeMetadata Type { get; set; }
        string? Name { get; set; }
    }

    public class ParameterMetadata : IParameterMetadata
    {
        public ParameterMetadata(int id) => Id = id;

        public int Id { get; }
        public ITypeMetadata Type { get; set; } = default!;
        public string? Name { get; set; }
    }
}