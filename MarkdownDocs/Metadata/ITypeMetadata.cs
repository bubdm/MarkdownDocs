using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public interface ITypeMetadata
    {
        int Id { get; }
        string Name { get; }
        string FullName { get; }
        string? Namespace { get; }
        string? Assembly { get; }
        string? Company { get; }

        AccessModifier AccessModifier { get; }
        TypeCategory Category { get; }
        TypeModifier Modifier { get; }

        ITypeMetadata? Inherited { get; }
        IEnumerable<ITypeMetadata> Implemented { get; }
        IEnumerable<ITypeMetadata> Derived { get; }
        IEnumerable<ITypeMetadata> References { get; }

        IEnumerable<IConstructorMetadata> Constructors { get; }
        IEnumerable<IPropertyMetadata> Properties { get; }
        IEnumerable<IMethodMetadata> Methods { get; }
    }
}