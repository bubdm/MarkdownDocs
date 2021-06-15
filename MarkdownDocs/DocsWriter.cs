using MarkdownDocs.Markdown;
using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs
{
    public class DocsWriter : IDocsWriter
    {
        private readonly IMarkdownWriter _markdownWriter;

        public DocsWriter(IMarkdownWriter markdownWriter) => _markdownWriter = markdownWriter;

        public async Task WriteAsync(IAssemblyMetadata metadata, DocsOptions options, CancellationToken cancellationToken)
        {

        }
    }
}
