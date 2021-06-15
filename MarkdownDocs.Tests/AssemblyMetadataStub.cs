using MarkdownDocs.Metadata;
using System.Collections.Generic;

namespace MarkdownDocs.Tests
{
    class AssemblyMetadataStub : IAssemblyMetadata
    {
        private readonly Dictionary<int, TypeMetadata> _types = new Dictionary<int, TypeMetadata>();
        public IEnumerable<TypeMetadata> Types => _types.Values;

        public TypeMetadata Type(int id)
        {
            if (_types.TryGetValue(id, out var meta))
            {
                return meta;
            }

            var newMeta = new TypeMetadata(id);
            _types.Add(id, newMeta);
            return newMeta;
        }
    }
}
