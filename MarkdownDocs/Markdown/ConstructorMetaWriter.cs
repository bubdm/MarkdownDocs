using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class ConstructorMetaWriter : IMetadataWriter<IConstructorMetadata>
    {
        private readonly IMarkdownWriter _writer;
        private readonly ISignatureFactory _signatureFactory;
        private readonly IDocsUrlResolver _urlResolver;
        private readonly IMetadataWriter<IParameterMetadata> _parameterWriter;

        public ConstructorMetaWriter(IMarkdownWriter writer, ISignatureFactory signatureFactory, IDocsUrlResolver urlResolver, Func<IMarkdownWriter, IMetadataWriter<IParameterMetadata>> parameterWriter)
        {
            _writer = writer;
            _signatureFactory = signatureFactory;
            _urlResolver = urlResolver;
            _parameterWriter = parameterWriter(writer);
        }

        public async Task WriteAsync(IConstructorMetadata constructor, uint indent, CancellationToken cancellationToken)
        {
            WriteTitle(constructor, indent);
            WriteSummary(constructor);
            WriteSignature(constructor);

            await WriteParametersAsync(constructor, indent, cancellationToken).ConfigureAwait(false);
        }

        private void WriteSignature(IConstructorMetadata constructor)
        {
            using (_writer.WriteCodeBlock())
            {
                string constructorSignature = _signatureFactory.CreateConstructor(constructor);
                _writer.Write(constructorSignature);
            }
        }

        private void WriteTitle(IConstructorMetadata constructor, uint indent)
        {
            string parameters = string.Join(", ", constructor.Parameters.Select(p => _urlResolver.GetTypeName(p.Type, constructor.Owner)));
            string name = constructor.Name.Sanitize();
            _writer.WriteHeading($"{name}({parameters})", indent);
        }

        private async Task WriteParametersAsync(IConstructorMetadata constructor, uint indent, CancellationToken cancellationToken)
        {
            List<IParameterMetadata> parameters = constructor.Parameters.ToList();
            if (parameters.Count > 0)
            {
                _writer.WriteLine("Parameters".Bold());

                foreach (IParameterMetadata parameter in parameters)
                {
                    await _parameterWriter.WriteAsync(parameter, indent, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private void WriteSummary(IConstructorMetadata method)
        {

        }
    }
}
