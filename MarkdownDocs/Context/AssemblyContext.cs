using MarkdownDocs.Metadata;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MarkdownDocs.Context
{
    public interface IAssemblyContext
    {
        string? Name { get; set; }
        ITypeContext Type(int id);
        IAssemblyMetadata GetMetadata();
    }

    public class AssemblyContext : IAssemblyMetadata, IAssemblyContext
    {
        private readonly ConcurrentDictionary<int, ITypeContext> _types = new ConcurrentDictionary<int, ITypeContext>(64, 64);
        private readonly Func<int, ITypeContext> _typeFactory = (id) => new TypeContext(id);

        public string? Name { get; set; }
        public IEnumerable<ITypeMetadata> Types => _types.Values.Select(t => t.GetMetadata());

        public ITypeContext Type(int id) => _types.GetOrAdd(id, _typeFactory);
        public IAssemblyMetadata GetMetadata() => this;
    }
}
