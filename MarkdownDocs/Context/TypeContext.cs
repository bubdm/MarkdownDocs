using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MarkdownDocs.Context
{
    [DebuggerDisplay("{FullName}")]
    public class TypeContext : ITypeContext, ITypeMetadata
    {
        private readonly Dictionary<int, IConstructorContext> _constructors = new Dictionary<int, IConstructorContext>();
        private readonly Dictionary<int, IFieldContext> _fields = new Dictionary<int, IFieldContext>();
        private readonly Dictionary<int, IPropertyContext> _properties = new Dictionary<int, IPropertyContext>();
        private readonly Dictionary<int, IMethodContext> _methods = new Dictionary<int, IMethodContext>();

        private readonly HashSet<ITypeMetadata> _implemented = new HashSet<ITypeMetadata>();
        private readonly HashSet<ITypeMetadata> _derived = new HashSet<ITypeMetadata>();
        private readonly HashSet<ITypeMetadata> _references = new HashSet<ITypeMetadata>();

        public ITypeMetadata? Inherited { get; private set; }
        public IEnumerable<ITypeMetadata> Implemented => _implemented;
        public IEnumerable<ITypeMetadata> Derived => _derived;
        public IEnumerable<ITypeMetadata> References => _references;

        public IEnumerable<IFieldMetadata> Fields => _fields.Values.Select(m => m.GetMetadata());
        public IEnumerable<IConstructorMetadata> Constructors => _constructors.Values.Select(m => m.GetMetadata());
        public IEnumerable<IPropertyMetadata> Properties => _properties.Values.Select(m => m.GetMetadata());
        public IEnumerable<IMethodMetadata> Methods => _methods.Values.Select(m => m.GetMetadata());

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

        public void Reference(ITypeContext context)
        {
            ITypeMetadata type = context.GetMetadata();
            if (this != type && type.Assembly == Assembly && _references.Add(type))
            {
                context.Reference(this);
            }
        }

        public void Inherit(ITypeContext context)
        {
            ITypeMetadata type = context.GetMetadata();
            if (Inherited != type)
            {
                Inherited = type;
                context.Derive(this);
            }
        }

        public void Implement(ITypeContext context)
        {
            ITypeMetadata type = context.GetMetadata();
            if (_implemented.Add(type))
            {
                context.Derive(this);
            }
        }

        public void Derive(ITypeContext context)
        {
            ITypeMetadata type = context.GetMetadata();
            if (_derived.Add(type))
            {
                if (type.Category == TypeCategory.Interface)
                {
                    context.Implement(this);
                }
                else
                {
                    context.Inherit(this);
                }
            }
        }

        public IConstructorContext Constructor(int id)
        {
            if (_constructors.TryGetValue(id, out var ctor))
            {
                return ctor;
            }

            var newCtor = new ConstructorContext(id, this);
            _constructors.Add(id, newCtor);

            return newCtor;
        }

        public IFieldContext Field(int id)
        {
            if (_fields.TryGetValue(id, out var field))
            {
                return field;
            }

            var newField = new FieldContext(id, this);
            _fields.Add(id, newField);

            return newField;
        }

        public IPropertyContext Property(int id)
        {
            if (_properties.TryGetValue(id, out var prop))
            {
                return prop;
            }

            var newProp = new PropertyContext(id, this);
            _properties.Add(id, newProp);

            return newProp;
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

        public ITypeMetadata GetMetadata()
            => this;

        public override int GetHashCode()
            => Id;

        public override bool Equals(object? obj)
            => obj is TypeContext other && other.Id == Id;
    }
}