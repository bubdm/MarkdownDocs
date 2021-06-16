using MarkdownDocs.Metadata;
using System.Collections.Generic;

namespace MarkdownDocs.Tests
{
    class AssemblyMetadataStub : IAssemblyContext
    {
        private readonly Dictionary<int, TypeContext> _types = new Dictionary<int, TypeContext>();
        public IEnumerable<ITypeMetadata> Types => _types.Values;

        public string? Name { get; private set; }

        public IAssemblyMetadata Build() => this;

        public ITypeContext Type(int id)
        {
            if (_types.TryGetValue(id, out var meta))
            {
                return meta;
            }

            var newMeta = new TypeContext(id);
            _types.Add(id, newMeta);
            return newMeta;
        }

        public IAssemblyContext WithName(string? assemblyName)
        {
            Name = assemblyName;
            return this;
        }
    }
}
