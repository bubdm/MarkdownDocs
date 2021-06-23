using MarkdownDocs.Context;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Resolver
{
    public interface IDocResolver
    {
        Task ResolveAsync(IAssemblyContext context, CancellationToken cancellationToken = default);
    }
}
