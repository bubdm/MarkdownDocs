using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public static class TypeWriter
    {
        public static async Task WriteAsync(this TypeMetadata type, IMarkdownWriter writer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

        }
    }
}
