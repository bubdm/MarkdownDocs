using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class EventMetaWriter : IMetadataWriter<IEventMetadata>
    {
        private readonly IMarkdownWriter _writer;
        private readonly ISignatureFactory _signatureFactory;
        private readonly IDocsUrlResolver _urlResolver;

        public EventMetaWriter(IMarkdownWriter writer, ISignatureFactory signatureFactory, IDocsUrlResolver urlResolver)
        {
            _writer = writer;
            _signatureFactory = signatureFactory;
            _urlResolver = urlResolver;
        }

        public Task WriteAsync(IEventMetadata ev, uint indent, CancellationToken cancellationToken)
        {
            WriteTitle(ev, indent);
            WriteSummary(ev);
            WriteSignature(ev);

            _writer.WriteLine("Event Type".Bold());
            string typeLink = ev.Type.Link(ev.Owner, _urlResolver);
            _writer.WriteLine(typeLink);

            return Task.CompletedTask;
        }

        private void WriteSignature(IEventMetadata ev)
        {
            using (_writer.WriteCodeBlock())
            {
                string evSignature = _signatureFactory.CreateEvent(ev);
                _writer.Write(evSignature);
            }
        }

        private void WriteTitle(IEventMetadata ev, uint indent) => _writer.WriteHeading(ev.Name, indent);

        private void WriteSummary(IEventMetadata ev)
        {

        }
    }
}
