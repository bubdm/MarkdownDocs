using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class MethodMetaWriter : IMetadataWriter<IMethodMetadata>
    {
        private readonly IMarkdownWriter _writer;
        private readonly ISignatureFactory _signatureFactory;
        private readonly IDocsUrlResolver _urlResolver;
        private readonly IMetadataWriter<IParameterMetadata> _parameterWriter;

        public MethodMetaWriter(IMarkdownWriter writer, ISignatureFactory signatureFactory, IDocsUrlResolver urlResolver, Func<IMarkdownWriter, IMetadataWriter<IParameterMetadata>> parameterWriter)
        {
            _writer = writer;
            _signatureFactory = signatureFactory;
            _urlResolver = urlResolver;
            _parameterWriter = parameterWriter(writer);
        }

        public async Task WriteAsync(IMethodMetadata method, uint indent, CancellationToken cancellationToken)
        {
            WriteTitle(method, indent);
            WriteSummary(method);
            WriteSignature(method);

            await WriteParametersAsync(method, indent, cancellationToken).ConfigureAwait(false);

            if (method.ReturnType.Name != typeof(void).Name)
            {
                _writer.WriteLine("Returns".Bold());
                string typeLink = method.ReturnType.Link(method.Owner, _urlResolver);
                _writer.Write(typeLink);

                if (!string.IsNullOrWhiteSpace(method.ReturnDescription))
                {
                    _writer.Write($": {method.ReturnDescription}");
                }
                _writer.WriteLine();
            }
        }

        private void WriteSignature(IMethodMetadata method)
        {
            using (_writer.WriteCodeBlock())
            {
                string methodSignature = _signatureFactory.CreateMethod(method);
                _writer.Write(methodSignature);
            }
        }

        private void WriteTitle(IMethodMetadata method, uint indent)
        {
            string parameters = string.Join(", ", method.Parameters.Select(p => _urlResolver.GetTypeName(p.Type, method.Owner)));
            string name = method.Name.Sanitize();
            _writer.WriteHeading($"{name}({parameters})", indent);
        }

        private async Task WriteParametersAsync(IMethodMetadata method, uint indent, CancellationToken cancellationToken)
        {
            List<IParameterMetadata> parameters = method.Parameters.ToList();
            if (parameters.Count > 0)
            {
                _writer.WriteLine("Parameters".Bold());

                foreach (IParameterMetadata parameter in parameters)
                {
                    await _parameterWriter.WriteAsync(parameter, indent, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private void WriteSummary(IMethodMetadata method)
        {
            if (!string.IsNullOrWhiteSpace(method.Description))
            {
                _writer.WriteLine(method.Description);
            }
        }
    }
}
