using MarkdownDocs.Metadata;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class MarkdownTypeWriter : IMarkdownWriterAsync<ITypeMetadata>
    {
        private readonly IMarkdownWriter _writer;
        private readonly IDocsOptions _options;
        private readonly uint _baseHeadingLevel;

        public MarkdownTypeWriter(IMarkdownWriter writer, IDocsOptions options)
        {
            _writer = writer;
            _options = options;
            _baseHeadingLevel = options.IsCompact ? 2u : 1u;
        }

        public async Task WriteAsync(ITypeMetadata type, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _writer.WriteHeading($"{type.Name} {type.Category}", _baseHeadingLevel);
            _writer.WriteLine($"{"Namespace:".Bold()} {type.Namespace}");
            _writer.WriteLine($"{"Assembly:".Bold()} {type.Assembly}");

            if (type.Category == TypeCategory.Class)
            {
                _writer.WriteLine($"{"Inheritance:".Bold()} {GetInheritanceChain(type)}");
            }
        }

        private string GetInheritanceChain(ITypeMetadata type)
        {
            var baseTypes = new List<string>();
            ITypeMetadata? current = type.Inherited;

            while (current != null)
            {
                baseTypes.Add(current.Link(type, _options));
                current = current.Inherited;
            }

            baseTypes.Reverse();
            string result = string.Join(" → ", baseTypes);
            return result;
        }
    }
}
