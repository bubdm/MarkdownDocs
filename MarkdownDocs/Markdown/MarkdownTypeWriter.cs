using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
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

            _writer.WriteLine($"{"Inheritance:".Bold()} {GetInheritanceChain(type)}");

            WriteImplementations(type);
            WriteDerivedTypes(type);
            WriteReferences(type);
        }

        private void WriteReferences(ITypeMetadata type)
        {
            string references = string.Join(", ", type.References.Select(i => i.Link(type, _options)));
            if (references.Length > 0)
            {
                _writer.WriteLine($"{"References:".Bold()} {references}");
            }
        }

        private void WriteDerivedTypes(ITypeMetadata type)
        {
            string derived = string.Join(", ", type.Derived.Select(i => i.Link(type, _options)));
            if (derived.Length > 0)
            {
                _writer.WriteLine($"{"Derived:".Bold()} {derived}");
            }
        }

        private void WriteImplementations(ITypeMetadata type)
        {
            string implementations = string.Join(", ", type.Implemented.Select(i => i.Link(type, _options)));
            if (implementations.Length > 0)
            {
                _writer.WriteLine($"{"Implements:".Bold()} {implementations}");
            }
        }

        private string GetInheritanceChain(ITypeMetadata type)
        {
            var baseTypes = new List<string>();
            ITypeMetadata? current = type;

            do
            {
                baseTypes.Add(current.Link(type, _options));
                current = current.Inherited;
            }
            while (current != null);

            baseTypes.Reverse();
            string result = string.Join(" → ", baseTypes);
            return result;
        }
    }
}
