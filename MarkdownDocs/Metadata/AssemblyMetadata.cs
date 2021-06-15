using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace MarkdownDocs.Metadata
{
    public class AssemblyMetadata : IAssemblyMetadata
    {
        public IEnumerable<TypeMetadata> Types => _types.Values;
        private readonly ConcurrentDictionary<int, TypeMetadata> _types = new ConcurrentDictionary<int, TypeMetadata>(64, 64);
        private readonly Func<int, TypeMetadata> _typeFactory = (id) => new TypeMetadata(id);

        public TypeMetadata Type(int id) => _types.GetOrAdd(id, _typeFactory);
    }
}
