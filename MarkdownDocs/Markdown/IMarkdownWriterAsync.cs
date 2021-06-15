using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public interface IMarkdownWriterAsync<T>
    {
        Task WriteAsync(T item, CancellationToken cancellationToken);
    }
}
