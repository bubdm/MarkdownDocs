using MarkdownDocs.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class ParameterMetaWriter : IMetadataWriter<IParameterMetadata>
    {
        private readonly IMarkdownWriter _writer;
        private readonly IDocsUrlResolver _urlResolver;

        public ParameterMetaWriter(IMarkdownWriter writer, IDocsUrlResolver urlResolver)
        {
            _writer = writer;
            _urlResolver = urlResolver;
        }

        public Task WriteAsync(IParameterMetadata parameter, uint indent, CancellationToken cancellationToken)
        {
            _writer.WriteCode(parameter.Name);
            _writer.Write(" ");

            string typeLink = parameter.Type.Link(parameter.Owner, _urlResolver);
            _writer.Write(typeLink);

            WriteSummary(parameter);
            _writer.WriteLine();

            return Task.CompletedTask;
        }

        private void WriteSummary(IParameterMetadata parameter)
        {
            if (!string.IsNullOrWhiteSpace(parameter.Description))
            {
                _writer.Write($": {parameter.Description}");
            }
        }
    }
}
