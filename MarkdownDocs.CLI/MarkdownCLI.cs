using MarkdownDocs.Markdown;
using MarkdownDocs.Metadata;
using MarkdownDocs.Resolver;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.CLI
{
    public interface IMarkdownCLI
    {
        Task WriteDocsAsync(CancellationToken cancellationToken = default);
    }

    public class MarkdownCLI : IMarkdownCLI
    {
        private readonly IAssemblyResolver _assemblyVisitor;
        private readonly IDocsWriter _docsWriter;

        public DocsOptions Options { get; }

        private MarkdownCLI(DocsOptions options, IAssemblyResolver assemblyVisitor, IDocsWriter docsWriter)
        {
            Options = options;
            _assemblyVisitor = assemblyVisitor;
            _docsWriter = docsWriter;
        }

        public async Task WriteDocsAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IAssemblyMetadata metadata = await _assemblyVisitor.ResolveAsync(Options, cancellationToken).ConfigureAwait(false);
            await _docsWriter.WriteAsync(metadata, Options, cancellationToken).ConfigureAwait(false);
        }

        public static IMarkdownCLI New(DocsOptions options)
        {
            var assemblyMeta = new AssemblyMetadata();
            var assemblyVisitor = new AssemblyResolver(assemblyMeta);

            static IMarkdownWriter WriterFactory(StreamWriter stream) => new MarkdownWriter(stream);
            var docsWriter = new DocsWriter(WriterFactory);

            return new MarkdownCLI(options, assemblyVisitor, docsWriter);
        }
    }
}
