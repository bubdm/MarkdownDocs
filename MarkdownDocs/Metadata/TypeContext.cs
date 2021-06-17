using System;
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
    public class TypeContext : ITypeContext
    {
        private readonly Dictionary<int, PropertyMetadata> _properties = new Dictionary<int, PropertyMetadata>();
        private readonly Dictionary<int, IMethodContext> _methods = new Dictionary<int, IMethodContext>();
        private readonly HashSet<ITypeContext> _implemented = new HashSet<ITypeContext>();
        private readonly HashSet<ITypeContext> _derived = new HashSet<ITypeContext>();
        private readonly HashSet<ITypeContext> _references = new HashSet<ITypeContext>();

        public ITypeContext? Inherited { get; private set; }
        public IEnumerable<ITypeContext> Implemented => _implemented;
        public IEnumerable<ITypeContext> Derived => _derived;
        public IEnumerable<ITypeContext> References => _references;

        public IEnumerable<IMethodMetadata> Methods => _methods.Values;
        public IEnumerable<PropertyMetadata> Properties => _properties.Values;

        public int Id { get; private set; }
        public string Name { get; set; } = default!;
        public string? Namespace { get; set; }
        public string? Assembly { get; set; } = default!;
        public string? Company { get; set; }
        public TypeCategory Category { get; set; }
        public TypeModifier Modifier { get; set; }
        public AccessModifier AccessModifier { get; set; } = AccessModifier.Public;
        public string FullName => Name.Contains("?") ? $"{Namespace}.{nameof(Nullable)}<{Name.Replace("?", string.Empty)}>" : $"{Namespace}.{Name}";

        public TypeContext(int id) => Id = id;

        public void Reference(ITypeContext type)
            => _references.Add(type);

        public void Inherit(ITypeContext type)
        {
            if (Inherited != type)
            {
                Inherited = type;
                type.Derive(this);
            }
        }

        public void Implement(ITypeContext type)
        {
            if (_implemented.Add(type))
            {
                type.Derive(this);
            }
        }

        public void Derive(ITypeContext type)
        {
            if (_derived.Add(type))
            {
                if (type.Category == TypeCategory.Interface)
                {
                    type.Implement(this);
                }
                else
                {
                    type.Inherit(this);
                }
            }
        }

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

        public IMethodContext Method(int id)
        {
            if (_methods.TryGetValue(id, out var method))
            {
                return method;
            }

            var newMethod = new MethodContext(id, this);
            _methods.Add(id, newMethod);

            return newMethod;
        }

        public EventMetadata Event(int id)
        {
            return default;
        }

        public ITypeContext Build() => this;

        public override int GetHashCode()
            => Id;

        public override bool Equals(object? obj)
            => obj is TypeContext other && other.Id == Id;
    }
}