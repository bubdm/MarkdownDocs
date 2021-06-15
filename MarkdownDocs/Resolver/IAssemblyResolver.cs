using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Resolver
{
    public interface IAssemblyResolver
    {
        Task<IAssemblyMetadata> ResolveAsync(IDocsOptions options, CancellationToken cancellationToken = default);
    }
}
