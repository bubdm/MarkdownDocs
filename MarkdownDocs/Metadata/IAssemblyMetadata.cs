using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface IAssemblyMetadata
    {
        IEnumerable<TypeMetadata> Types { get; }

        TypeMetadata Type(int id);
    }
}