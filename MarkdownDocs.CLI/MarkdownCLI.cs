using MarkdownDocs.Markdown;
using MarkdownDocs.Metadata;
using MarkdownDocs.Resolver;
using System.Threading.Tasks;

namespace MarkdownDocs.CLI
{
    public interface IMarkdownCLI
    {
        Task WriteDocsAsync();
    }

    public class MarkdownCLI : IMarkdownCLI
    {
        private readonly IAssemblyResolver _assemblyVisitor;
        private readonly IDocsWriter _docsWriter;

        public Options Options { get; }

        public MarkdownCLI(Options options, IAssemblyResolver assemblyVisitor, IDocsWriter docsWriter)
        {
            Options = options;
            _assemblyVisitor = assemblyVisitor;
            _docsWriter = docsWriter;
        }

        public async Task WriteDocsAsync()
        {
            AssemblyMetadata metadata = await _assemblyVisitor.ResolveAsync(Options.InputPath);
            await _docsWriter.WriteAsync(metadata, Options);
        }

        public static IMarkdownCLI New(Options options)
        {
            var metaBuilder = new MetadataBuilder();
            var markdownWriter = new MarkdownWriter();
            var assemblyVisitor = new AssemblyResolver(metaBuilder);
            var docsWriter = new DocsWriter(markdownWriter);

            return new MarkdownCLI(options, assemblyVisitor, docsWriter);
        }
    }
}
