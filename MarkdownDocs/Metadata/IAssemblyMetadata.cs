using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface IAssemblyMetadata
    {
        string? Name { get; }

        IEnumerable<ITypeMetadata> Types { get; }
    }
}