using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Resolver
{
    public interface IAssemblyResolver
    {
        Task<IAssemblyMetadata> ResolveAsync(DocsOptions options, CancellationToken cancellationToken = default);
    }
}
