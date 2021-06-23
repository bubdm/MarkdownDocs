using MarkdownDocs.Context;
using MarkdownDocs.Markdown;
using MarkdownDocs.Metadata;
using MarkdownDocs.Resolver;
using System;
using System.Collections.Generic;
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
        private readonly ICollection<IDocResolver> _assemblyResolvers;
        private readonly IDocsWriter _docsWriter;
        private readonly Func<IAssemblyContext> _assemblyContextFactory;

        public IDocsOptions Options { get; }

        private MarkdownCLI(IDocsOptions options, IDocsWriter docsWriter, Func<IAssemblyContext> assemblyContextFactory, ICollection<IDocResolver> assemblyResolvers)
        {
            Options = options;
            _assemblyResolvers = assemblyResolvers;
            _docsWriter = docsWriter;
            _assemblyContextFactory = assemblyContextFactory;
        }

        public async Task WriteDocsAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IAssemblyContext context = _assemblyContextFactory();

            foreach (var assemblyResolver in _assemblyResolvers)
            {
                await assemblyResolver.ResolveAsync(context, cancellationToken).ConfigureAwait(false);
            }

            IAssemblyMetadata metadata = context.GetMetadata();
            await _docsWriter.WriteAsync(metadata, Options, cancellationToken).ConfigureAwait(false);
        }

        public static IMarkdownCLI New(IDocsOptions options)
        {
            IDocsUrlResolver urlResolver = new DocsUrlResolver(options);
            ISignatureFactory signatureFactory = new SignatureFactory(options, urlResolver);
            IDocResolver assemblyResolver = new AssemblyResolver(options, TypeResolverFactory, MethodResolverFactory, ConstructorResolverFactory, FieldResolverFactory, PropertyResolverFactory, EventResolverFactory);
            IXMLMemberResolver xmlMemberResolver = new XMLMemberResolver(urlResolver);
            IDocResolver xmlResolver = new XMLResolver(options, xmlMemberResolver);

            var resolvers = new List<IDocResolver>
            {
                assemblyResolver
            };

            if (options.UseXML)
            {
                resolvers.Add(xmlResolver);
            }

            IDocsWriter docsWriter = new DocsWriter(WriterFactory, TypeWriterFactory);
            return new MarkdownCLI(options, docsWriter, ContextFactoy, resolvers);

            IMetadataWriter<IEventMetadata> EventWriterFactory(IMarkdownWriter writer) => new EventMetaWriter(writer, signatureFactory, urlResolver);
            IMetadataWriter<IPropertyMetadata> PropertyWriterFactory(IMarkdownWriter writer) => new PropertyMetaWriter(writer, signatureFactory, urlResolver);
            IMetadataWriter<IFieldMetadata> FieldWriterFactory(IMarkdownWriter writer) => new FieldMetaWriter(writer, signatureFactory, urlResolver);
            IMetadataWriter<IParameterMetadata> ParameterWriterFactory(IMarkdownWriter writer) => new ParameterMetaWriter(writer, urlResolver);
            IMetadataWriter<IConstructorMetadata> ConstructorWriterFactory(IMarkdownWriter writer) => new ConstructorMetaWriter(writer, signatureFactory, urlResolver, ParameterWriterFactory);
            IMetadataWriter<IMethodMetadata> MethodWriterFactory(IMarkdownWriter writer) => new MethodMetaWriter(writer, signatureFactory, urlResolver, ParameterWriterFactory);
            IMetadataWriter<ITypeMetadata> TypeWriterFactory(IMarkdownWriter writer) => new TypeMetaWriter(writer, signatureFactory, urlResolver, MethodWriterFactory, ConstructorWriterFactory, FieldWriterFactory, PropertyWriterFactory, EventWriterFactory, ParameterWriterFactory);

            static IAssemblyContext ContextFactoy() => new AssemblyContext();

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
