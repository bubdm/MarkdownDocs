using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public class AssemblyMetadata : IAssemblyMetadata, IAssemblyBuilder
    {
        private readonly ConcurrentDictionary<int, TypeMetadata> _types = new ConcurrentDictionary<int, TypeMetadata>(64, 64);
        private readonly Func<int, TypeMetadata> _typeFactory = (id) => new TypeMetadata(id);
        
        public IEnumerable<ITypeMetadata> Types => _types.Values;
        public string? Name { get; private set; } = "UNKNOWN";

        public TypeMetadata Type(int id) => _types.GetOrAdd(id, _typeFactory);

        public IAssemblyMetadata Build() => this;

        public IAssemblyBuilder WithName(string? assemblyName)
        {
            Name = assemblyName;
            return this;
        }
    }
}
