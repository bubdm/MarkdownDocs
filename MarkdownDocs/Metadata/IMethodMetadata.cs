using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface IMethodMetadata : IMemberMetadata
    {
        public IEnumerable<IParameterMetadata> Parameters { get; }
        public ITypeMetadata ReturnType { get; }
    }
}