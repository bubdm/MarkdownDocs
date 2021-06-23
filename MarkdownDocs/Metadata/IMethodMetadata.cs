using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface IParameterizedMemberMetadata : IDocMetadata
    {
        IEnumerable<IParameterMetadata> Parameters { get; }
    }

    public interface IMethodMetadata : IMemberMetadata, IParameterizedMemberMetadata
    {
        string? ReturnDescription { get; }
        ITypeMetadata ReturnType { get; }
        MethodModifier MethodModifier { get; }
    }
}