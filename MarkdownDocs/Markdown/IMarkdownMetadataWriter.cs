using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public interface IMarkdownMetadataWriter<T>
    {
        Task WriteAsync(T item, CancellationToken cancellationToken);
    }
}
