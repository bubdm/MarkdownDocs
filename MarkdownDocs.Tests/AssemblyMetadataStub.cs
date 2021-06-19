using MarkdownDocs.Context;
using MarkdownDocs.Metadata;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownDocs.Tests
{
    class AssemblyMetadataStub : IAssemblyMetadata, IAssemblyContext
    {
        private readonly Dictionary<int, ITypeContext> _types = new Dictionary<int, ITypeContext>();
        public IEnumerable<ITypeMetadata> Types => _types.Values.Select(t => t.GetMetadata());

        public string? Name { get; private set; }

        public IAssemblyMetadata GetMetadata() => this;

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
