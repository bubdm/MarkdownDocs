using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class TypeMetaWriter : IMetadataWriter<ITypeMetadata>
    {
        private readonly IMarkdownWriter _writer;
        private readonly ISignatureFactory _signatureFactory;
        private readonly IDocsUrlResolver _urlResolver;
        private readonly IMetadataWriter<IMethodMetadata> _methodWriter;

        public TypeMetaWriter(IMarkdownWriter writer, ISignatureFactory signatureFactory, IDocsUrlResolver urlResolver, Func<IMarkdownWriter, IMetadataWriter<IMethodMetadata>> methodWriterFactory)
        {
            _writer = writer;
            _signatureFactory = signatureFactory;
            _urlResolver = urlResolver;
            _methodWriter = methodWriterFactory(writer);
        }

        public async Task WriteAsync(ITypeMetadata type, uint indent, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            _writer.WriteHeading($"{type.Name} {type.Category}", indent);
            _writer.WriteLine($"{"Namespace:".Bold()} {type.Namespace}");
            _writer.WriteLine($"{"Assembly:".Bold()} {type.Assembly}");

            _writer.WriteLine($"{"Inheritance:".Bold()} {GetInheritanceChain(type)}");

            WriteImplementations(type);
            WriteDerivedTypes(type);
            WriteReferences(type);

            WriteSummary(type);
            WriteSignature(type);

            if (type.Category != TypeCategory.Delegate)
            {
                await WriteMethodsAsync(type, indent + 1, cancellationToken);
            }
        }

        private void WriteSummary(ITypeMetadata type)
        {

        }

        private void WriteSignature(ITypeMetadata type)
        {
            using (_writer.WriteCodeBlock())
            {
                string typeSignature = _signatureFactory.CreateType(type);
                _writer.Write(typeSignature);
            }
        }

        private void WriteReferences(ITypeMetadata type)
        {
            string references = string.Join(", ", type.References.Select(i => i.Link(type, _urlResolver)));
            if (references.Length > 0)
            {
                _writer.WriteLine($"{"References:".Bold()} {references}");
            }
        }

        private void WriteDerivedTypes(ITypeMetadata type)
        {
            string derived = string.Join(", ", type.Derived.Select(i => i.Link(type, _urlResolver)));
            if (derived.Length > 0)
            {
                _writer.WriteLine($"{"Derived:".Bold()} {derived}");
            }
        }

        private void WriteImplementations(ITypeMetadata type)
        {
            string implementations = string.Join(", ", type.Implemented.Select(i => i.Link(type, _urlResolver)));
            if (implementations.Length > 0)
            {
                _writer.WriteLine($"{"Implements:".Bold()} {implementations}");
            }
        }

        private async Task WriteMethodsAsync(ITypeMetadata type, uint indent, CancellationToken cancellationToken)
        {
            List<IMethodMetadata> methods = type.Methods.OrderBy(m => m.Name).ToList();
            if (methods.Count > 0)
            {
                _writer.WriteHeading("Methods", indent);

                foreach (IMethodMetadata method in methods)
                {
                    await _methodWriter.WriteAsync(method, indent + 1, cancellationToken);
                }
            }
        }

        private string GetInheritanceChain(ITypeMetadata type)
        {
            var baseTypes = new List<string>();
            ITypeMetadata? current = type;

            do
            {
                baseTypes.Add(current.Link(type, _urlResolver));
                current = current.Inherited;
            }
            while (current != null);

            baseTypes.Reverse();
            string result = string.Join(" → ", baseTypes);
            return result;
        }
    }
}
