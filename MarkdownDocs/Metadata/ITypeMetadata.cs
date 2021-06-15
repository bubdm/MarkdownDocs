using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface ITypeMetadata
    {
        int Id { get; }
        string? Assembly { get; set; }
        string Name { get; set; }
        string? Namespace { get; set; }
        bool IsMicrosoftType { get; set; }

        TypeCategory Category { get; set; }
        TypeModifier Modifier { get; set; }
        IEnumerable<TypeMetadata> Derived { get; }
        IEnumerable<TypeMetadata> Inherited { get; }
        IEnumerable<TypeMetadata> References { get; }
    }
}