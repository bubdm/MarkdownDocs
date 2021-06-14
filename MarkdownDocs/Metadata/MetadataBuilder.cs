using System;
using System.Collections.Concurrent;

namespace MarkdownDocs.Metadata
{
    public class MetadataBuilder : IMetadataBuilder
    {
        private readonly ConcurrentDictionary<int, TypeMetadata> _types = new ConcurrentDictionary<int, TypeMetadata>(64, 64);
        private readonly Func<int, TypeMetadata> _typeFactory = (id) => new TypeMetadata(id);

        public AssemblyMetadata Build()
        {
            return default;
        }

        public TypeMetadata Type(int id) => _types.GetOrAdd(id, _typeFactory);
    }
}
