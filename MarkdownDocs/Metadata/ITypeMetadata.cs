using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface ITypeMetadata
    {
        int Id { get; }
        string Name { get; set; }
        string? Namespace { get; set; }
        string? Assembly { get; set; }
        bool IsMicrosoftType { get; set; }

        TypeCategory Category { get; set; }
        TypeModifier Modifier { get; set; }
        ITypeMetadata? Inherited { get; }
        IEnumerable<ITypeMetadata> Implemented { get; }
        IEnumerable<ITypeMetadata> Derived { get; }
        IEnumerable<ITypeMetadata> References { get; }
    }
}