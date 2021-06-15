using System.Collections.Generic;
using System.Diagnostics;

namespace MarkdownDocs.Metadata
{
    public enum TypeCategory
    {
        Unknown,
        Enum,
        Interface,
        Struct,
        Delegate,
        Class
    }

    public enum TypeModifier
    {
        None,
        Abstract,
        Sealed,
        Static
    }

    [DebuggerDisplay("{FullName}")]
    public class TypeMetadata : ITypeMetadata, ITypeBuilder
    {
        private readonly Dictionary<int, PropertyMetadata> _properties = new Dictionary<int, PropertyMetadata>();
        private readonly HashSet<ITypeMetadata> _implemented = new HashSet<ITypeMetadata>();
        private readonly HashSet<ITypeMetadata> _derived = new HashSet<ITypeMetadata>();
        private readonly HashSet<ITypeMetadata> _references = new HashSet<ITypeMetadata>();

        public ITypeMetadata? Inherited { get; private set; }
        public IEnumerable<ITypeMetadata> Implemented => _implemented;
        public IEnumerable<ITypeMetadata> Derived => _derived;
        public IEnumerable<ITypeMetadata> References => _references;

        public int Id { get; private set; }
        public string Name { get; set; } = default!;
        public string? Namespace { get; set; }
        public string? Assembly { get; set; } = default!;
        public bool IsMicrosoftType { get; set; }
        public TypeCategory Category { get; set; }
        public TypeModifier Modifier { get; set; }
        public string FullName => $"{Namespace}.{Name}";

        public TypeMetadata(int id) => Id = id;

        public void Reference(TypeMetadata type)
            => _references.Add(type);

        public void Inherit(TypeMetadata type)
        {
            Inherited = type;
            type.Derive(this);
        }

        public void Implement(TypeMetadata type)
        {
            _implemented.Add(type);
            type.Derive(this);
        }

        protected void Derive(TypeMetadata type) => _derived.Add(type);

        public PropertyMetadata Property(int id)
        {
            if (_properties.TryGetValue(id, out var prop))
            {
                return prop;
            }

            var newProp = new PropertyMetadata(id, this);
            _properties.Add(id, newProp);

            return newProp;
        }

        public ConstructorMetadata Constructor(int id)
        {
            return default;
        }

        public FieldMetadata Field(int id)
        {
            return default;
        }

        public MethodMetadata Method(int id)
        {
            return default;
        }

        public EventMetadata Event(int id)
        {
            return default;
        }

        public override int GetHashCode()
            => Id;

        public override bool Equals(object? obj)
            => obj is TypeMetadata other && other.Id == Id;
    }
}