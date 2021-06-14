using MarkdownDocs.Markdown;
using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs
{
    public class DocsWriter : IDocsWriter
    {
        private readonly MarkdownWriter _markdownWriter;

        public DocsWriter(MarkdownWriter markdownWriter) => _markdownWriter = markdownWriter;

        public async Task WriteAsync(AssemblyMetadata metadata, DocsOptions options, CancellationToken cancellationToken)
        {

        }
    }
}
