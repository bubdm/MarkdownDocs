using MarkdownDocs.Metadata;
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

            WriteSummary(type);
            WriteSignature(type);
        }

        private void WriteSummary(ITypeMetadata type)
        {

        }

        // TODO: Extract this logic into ISignatureProvider
        private void WriteSignature(ITypeMetadata type)
        {
            using (_writer.WriteCode())
            {
                if (type.Category == TypeCategory.Delegate)
                {
                    IMethodMetadata? method = type.Methods.FirstOrDefault(m => m.Name == "Invoke");
                    if (method != null)
                    {
                        // TODO: Extract this logic into ISignatureProvider
                        string parameters = string.Join(", ", method.Parameters.Select(p => $"{p.Type.GetName(_options)} {p.Name}"));
                        _writer.Write($"{method.AccessModifier.ToMarkdown()} {type.Category.ToMarkdown()} {type.Name}({parameters})".Clean());
                    }
                }
                else
                {
                    WriteTitle(type);

                    if (type.Category != TypeCategory.Enum)
                    {
                        string implemented = string.Join(", ", type.Implemented.Select(t => t.GetName(_options)));
                        bool inherits = type.Inherited != null && type.Inherited.Inherited != null;

                        if (inherits || implemented.Length > 0)
                        {
                            _writer.Write(" : ");
                        }

                        if (inherits)
                        {
                            _writer.Write($"{type.Inherited!.GetName(_options)}");
                        }

                        if (implemented.Length > 0)
                        {
                            _writer.Write($"{(inherits ? ", " : string.Empty)}{implemented}");
                        }
                    }
                }
            }

            void WriteTitle(ITypeMetadata type) => _writer.Write($"{type.AccessModifier.ToMarkdown()} {type.Modifier.ToMarkdown()} {type.Category.ToMarkdown()} {type.Name}".Clean());
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
