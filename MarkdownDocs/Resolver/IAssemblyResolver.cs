using MarkdownDocs.Context;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Resolver
{
    public interface IAssemblyResolver
    {
        Task<IAssemblyContext> ResolveAsync(IDocsOptions options, CancellationToken cancellationToken = default);
    }
}
