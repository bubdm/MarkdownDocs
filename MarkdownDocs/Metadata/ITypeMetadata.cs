using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface ITypeMetadata
    {
        int Id { get; }
        string Name { get; set; }
        string FullName { get; }
        string? Namespace { get; set; }
        string? Assembly { get; set; }
        string? Company { get; set; }

        AccessModifier AccessModifier { get; set; }
        TypeCategory Category { get; set; }
        TypeModifier Modifier { get; set; }

        ITypeContext? Inherited { get; }
        IEnumerable<ITypeContext> Implemented { get; }
        IEnumerable<ITypeContext> Derived { get; }
        IEnumerable<ITypeContext> References { get; }

        IEnumerable<IMethodMetadata> Methods { get; }
        IEnumerable<PropertyMetadata> Properties { get; }
    }
}