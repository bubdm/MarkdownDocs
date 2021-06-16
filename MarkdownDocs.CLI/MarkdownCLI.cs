using MarkdownDocs.Markdown;
using MarkdownDocs.Metadata;
using MarkdownDocs.Resolver;
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
        private readonly IAssemblyResolver _assemblyResolver;
        private readonly IDocsWriter _docsWriter;

        public IDocsOptions Options { get; }

        private MarkdownCLI(IDocsOptions options, IAssemblyResolver assemblyResolver, IDocsWriter docsWriter)
        {
            Options = options;
            _assemblyResolver = assemblyResolver;
            _docsWriter = docsWriter;
        }

        public async Task WriteDocsAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IAssemblyMetadata metadata = await _assemblyResolver.ResolveAsync(Options, cancellationToken).ConfigureAwait(false);
            await _docsWriter.WriteAsync(metadata, Options, cancellationToken).ConfigureAwait(false);
        }

        public static IMarkdownCLI New(IDocsOptions options)
        {
            var assemblyMeta = new AssemblyMetadata();
            var assemblyResolver = new AssemblyResolver(assemblyMeta, TypeResolverFactory, MethodResolverFactory);

            var docsWriter = new DocsWriter(WriterFactory, TypeWriterFactory);

            return new MarkdownCLI(options, assemblyResolver, docsWriter);

            static IMethodResolver MethodResolverFactory(ITypeResolver typeResolver, ITypeContext context) => new MethodResolver(typeResolver, context);
            static ITypeResolver TypeResolverFactory(IAssemblyContext builder) => new TypeResolver(builder, MethodResolverFactory);

            static IMarkdownWriter WriterFactory(StreamWriter stream) => new MarkdownWriter(stream);
            IMarkdownWriterAsync<ITypeMetadata> TypeWriterFactory(IMarkdownWriter writer) => new MarkdownTypeWriter(writer, options);
        }
    }
}
