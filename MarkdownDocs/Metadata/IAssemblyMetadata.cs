using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface IAssemblyMetadata
    {
        public string? Name { get; }

        IEnumerable<TypeMetadata> Types { get; }
    }
}