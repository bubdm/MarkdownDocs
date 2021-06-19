using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface IConstructorMetadata : IMemberMetadata
    {
        IEnumerable<IParameterMetadata> Parameters { get; }
    }
}
