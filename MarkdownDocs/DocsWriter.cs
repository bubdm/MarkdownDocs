using MarkdownDocs.Markdown;
using MarkdownDocs.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs
{
    public class DocsWriter : IDocsWriter
    {
        private readonly IMarkdownWriter _markdownWriter;

        public DocsWriter(IMarkdownWriter markdownWriter) => _markdownWriter = markdownWriter;

        public async Task WriteAsync(IAssemblyMetadata assembly, DocsOptions options, CancellationToken cancellationToken)
        {
            IEnumerable<TypeMetadata>? exportedTypes = assembly.Types.Where(t => t.Assembly == assembly.Name);

        }
    }
}
