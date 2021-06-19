using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public interface IMetadataWriter<T>
    {
        Task WriteAsync(T metadata, uint indent, CancellationToken cancellationToken);
    }
}
