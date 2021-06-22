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
            IAssemblyContext context = await _assemblyResolver.ResolveAsync(Options, cancellationToken).ConfigureAwait(false);
            IAssemblyMetadata metadata = context.GetMetadata();
            await _docsWriter.WriteAsync(metadata, Options, cancellationToken).ConfigureAwait(false);
        }

        public static IMarkdownCLI New(IDocsOptions options)
        {
            IDocsUrlResolver urlResolver = new DocsUrlResolver(options);
            IAssemblyContext assemblyContext = new AssemblyContext();
            IAssemblyResolver assemblyResolver = new AssemblyResolver(assemblyContext, TypeResolverFactory, MethodResolverFactory, ConstructorResolverFactory, FieldResolverFactory, PropertyResolverFactory, EventResolverFactory);
            ISignatureFactory signatureFactory = new SignatureFactory(options, urlResolver);

            IDocsWriter docsWriter = new DocsWriter(WriterFactory, TypeWriterFactory);
            return new MarkdownCLI(options, assemblyResolver, docsWriter);

            IMetadataWriter<IEventMetadata> EventWriterFactory(IMarkdownWriter writer) => new EventMetaWriter(writer, signatureFactory, urlResolver);
            IMetadataWriter<IPropertyMetadata> PropertyWriterFactory(IMarkdownWriter writer) => new PropertyMetaWriter(writer, signatureFactory, urlResolver);
            IMetadataWriter<IFieldMetadata> FieldWriterFactory(IMarkdownWriter writer) => new FieldMetaWriter(writer, signatureFactory, urlResolver);
            IMetadataWriter<IParameterMetadata> ParameterWriterFactory(IMarkdownWriter writer) => new ParameterMetaWriter(writer, urlResolver);
            IMetadataWriter<IConstructorMetadata> ConstructorWriterFactory(IMarkdownWriter writer) => new ConstructorMetaWriter(writer, signatureFactory, urlResolver, ParameterWriterFactory);
            IMetadataWriter<IMethodMetadata> MethodWriterFactory(IMarkdownWriter writer) => new MethodMetaWriter(writer, signatureFactory, urlResolver, ParameterWriterFactory);
            IMetadataWriter<ITypeMetadata> TypeWriterFactory(IMarkdownWriter writer) => new TypeMetaWriter(writer, signatureFactory, urlResolver, MethodWriterFactory, ConstructorWriterFactory, FieldWriterFactory, PropertyWriterFactory, EventWriterFactory);

            static IMarkdownWriter WriterFactory(StreamWriter stream) => new MarkdownWriter(stream);
            static IParameterResolver ParameterResolverFactory(IMethodBaseContext context, ITypeResolver typeResolver) => new ParameterResolver(context, typeResolver);
            static IEventResolver EventResolverFactory(ITypeContext context, ITypeResolver typeResolver) => new EventResolver(context, typeResolver);
            static IPropertyResolver PropertyResolverFactory(ITypeContext context, ITypeResolver typeResolver) => new PropertyResolver(context, typeResolver);
            static IFieldResolver FieldResolverFactory(ITypeContext context, ITypeResolver typeResolver) => new FieldResolver(context, typeResolver);
            static IConstructorResolver ConstructorResolverFactory(ITypeContext context, ITypeResolver typeResolver) => new ConstructorResolver(context, typeResolver, ParameterResolverFactory);
            static IMethodResolver MethodResolverFactory(ITypeContext context, ITypeResolver typeResolver) => new MethodResolver(context, typeResolver, ParameterResolverFactory);
            static ITypeResolver TypeResolverFactory(IAssemblyContext builder) => new TypeResolver(builder, MethodResolverFactory);
        }
    }
}
