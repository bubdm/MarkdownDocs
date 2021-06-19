using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class FieldMetaWriter : IMetadataWriter<IFieldMetadata>
    {
        private readonly IMarkdownWriter _writer;
        private readonly ISignatureFactory _signatureFactory;
        private readonly IDocsUrlResolver _urlResolver;

        public FieldMetaWriter(IMarkdownWriter writer, ISignatureFactory signatureFactory, IDocsUrlResolver urlResolver)
        {
            _writer = writer;
            _signatureFactory = signatureFactory;
            _urlResolver = urlResolver;
        }

        public Task WriteAsync(IFieldMetadata field, uint indent, CancellationToken cancellationToken)
        {
            WriteTitle(field, indent);
            WriteSummary(field);
            WriteSignature(field);

            _writer.WriteLine("Field Value".Bold());
            string typeLink = field.Type.Link(field.Owner, _urlResolver);
            _writer.WriteLine(typeLink);

            return Task.CompletedTask;
        }

        private void WriteSignature(IFieldMetadata field)
        {
            using (_writer.WriteCodeBlock())
            {
                string methodSignature = _signatureFactory.CreateField(field);
                _writer.Write(methodSignature);
            }
        }

        private void WriteTitle(IFieldMetadata field, uint indent) => _writer.WriteHeading($"{field.Name}", indent);

        private void WriteSummary(IFieldMetadata field)
        {

        }
    }
}
