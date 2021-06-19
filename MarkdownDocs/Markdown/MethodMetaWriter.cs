using MarkdownDocs.Metadata;
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

        public MethodMetaWriter(IMarkdownWriter writer, ISignatureFactory signatureFactory, IDocsUrlResolver urlResolver)
        {
            _writer = writer;
            _signatureFactory = signatureFactory;
            _urlResolver = urlResolver;
        }

        public async Task WriteAsync(IMethodMetadata method, uint indent, CancellationToken cancellationToken)
        {
            WriteTitle(method, indent);
            WriteSummary(method);
            WriteSignature(method);

            await WriteParameters(method);

            if (method.ReturnType.Name != typeof(void).Name)
            {
                _writer.WriteLine("Returns".Bold());
                string typeLink = method.ReturnType.Link(method.Owner, _urlResolver);
                _writer.WriteLine(typeLink);
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

        private async Task WriteParameters(IMethodMetadata method)
        {
            List<IParameterMetadata> parameters = method.Parameters.ToList();
            if (parameters.Count > 0)
            {
                _writer.WriteLine("Parameters".Bold());

                foreach (IParameterMetadata parameter in parameters)
                {
                    _writer.WriteCode(parameter.Name);
                    _writer.Write(" ");

                    string typeLink = parameter.Type.Link(method.Owner, _urlResolver);
                    _writer.WriteLine(typeLink);

                    WriteSummary(parameter);
                }
            }
        }

        private void WriteSummary(IParameterMetadata parameter)
        {

        }

        private void WriteSummary(IMethodMetadata method)
        {

        }
    }
}
