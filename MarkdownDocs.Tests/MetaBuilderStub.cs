using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;

namespace MarkdownDocs.Tests
{
    class MetaBuilderStub : IMetadataBuilder
    {
        private readonly Dictionary<int, TypeMetadata> _types = new Dictionary<int, TypeMetadata>();

        public AssemblyMetadata Build()
        {
            throw new NotImplementedException();
        }

        public TypeMetadata Type(int id)
        {
            if(_types.TryGetValue(id, out var meta))
            {
                return meta;
            }

            var newMeta = new TypeMetadata(id);
            _types.Add(id, newMeta);
            return newMeta;
        }
    }
}
