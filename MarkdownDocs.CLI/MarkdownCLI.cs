using MarkdownDocs.Context;
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
            IDocsUrlResolver urlResolver = new DocsUrlResolver(options);
            IAssemblyContext assemblyContext = new AssemblyContext();
            IAssemblyResolver assemblyResolver = new AssemblyResolver(assemblyContext, TypeResolverFactory, MethodResolverFactory);
            ISignatureFactory signatureFactory = new SignatureFactory(options, urlResolver);

            IDocsWriter docsWriter = new DocsWriter(WriterFactory, TypeWriterFactory);
            return new MarkdownCLI(options, assemblyResolver, docsWriter);

            static IMarkdownWriter WriterFactory(StreamWriter stream) => new MarkdownWriter(stream);
            static IParameterResolver ParameterResolverFactory(IMethodContext context, ITypeResolver typeResolver) => new ParameterResolver(context, typeResolver);
            static IMethodResolver MethodResolverFactory(ITypeResolver typeResolver, ITypeContext context) => new MethodResolver(typeResolver, context, ParameterResolverFactory);
            static ITypeResolver TypeResolverFactory(IAssemblyContext builder) => new TypeResolver(builder, MethodResolverFactory);
            IMarkdownMetadataWriter<ITypeMetadata> TypeWriterFactory(IMarkdownWriter writer) => new MarkdownTypeWriter(writer, signatureFactory, urlResolver, options);
        }
    }
}
