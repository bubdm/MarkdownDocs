using MarkdownDocs.Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MarkdownDocs.Context
{
    public class AssemblyContext : IAssemblyMetadata, IAssemblyContext
    {
        private readonly ConcurrentDictionary<int, ITypeContext> _types = new ConcurrentDictionary<int, ITypeContext>(64, 64);
        private readonly Func<int, ITypeContext> _typeFactory = (id) => new TypeContext(id);
        
        public IEnumerable<ITypeMetadata> Types => _types.Values;
        public string? Name { get; private set; } = "UNKNOWN";

        public ITypeContext Type(int id) => _types.GetOrAdd(id, _typeFactory);

        public IAssemblyMetadata Build() => this;

        public IAssemblyContext WithName(string? assemblyName)
        {
            Name = assemblyName;
            return this;
        }
    }
}
