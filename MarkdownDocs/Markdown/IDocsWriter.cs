using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public interface IDocsWriter
    {
        Task WriteAsync(IAssemblyMetadata metadata, IDocsOptions options, CancellationToken cancellationToken = default);
    }
}
