using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs
{
    public interface IDocsWriter
    {
        Task WriteAsync(AssemblyMetadata metadata, DocsOptions options, CancellationToken cancellationToken = default);
    }
}
