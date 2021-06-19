using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface IMethodMetadata : IMemberMetadata
    {
        IEnumerable<IParameterMetadata> Parameters { get; }
        ITypeMetadata ReturnType { get; }
        MethodModifier MethodModifier { get; set; }
    }
}