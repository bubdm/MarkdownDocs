using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class PropertyMetaWriter : IMetadataWriter<IPropertyMetadata>
    {
        private readonly IMarkdownWriter _writer;
        private readonly ISignatureFactory _signatureFactory;
        private readonly IDocsUrlResolver _urlResolver;

        public PropertyMetaWriter(IMarkdownWriter writer, ISignatureFactory signatureFactory, IDocsUrlResolver urlResolver)
        {
            _writer = writer;
            _signatureFactory = signatureFactory;
            _urlResolver = urlResolver;
        }

        public Task WriteAsync(IPropertyMetadata property, uint indent, CancellationToken cancellationToken)
        {
            WriteTitle(property, indent);
            WriteSummary(property);
            WriteSignature(property);

            _writer.WriteLine("Property Value".Bold());
            string typeLink = property.Type.Link(property.Owner, _urlResolver);
            _writer.WriteLine(typeLink);

            return Task.CompletedTask;
        }

        private void WriteSignature(IPropertyMetadata property)
        {
            using (_writer.WriteCodeBlock())
            {
                string propertySignature = _signatureFactory.CreateProperty(property);
                _writer.Write(propertySignature);
            }
        }

        private void WriteTitle(IPropertyMetadata property, uint indent) => _writer.WriteHeading(property.Name, indent);

        private void WriteSummary(IPropertyMetadata property)
        {
            if (!string.IsNullOrWhiteSpace(property.Description))
            {
                _writer.WriteLine(property.Description);
            }
        }
    }
}
