using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Resolver
{
    public interface IAssemblyResolver
    {
        Task<AssemblyMetadata> ResolveAsync(string path, CancellationToken cancellationToken = default);
    }
}
