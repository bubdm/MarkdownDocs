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
        private readonly ISignatureFactory _signatureFactory;
        private readonly IDocsUrlResolver _urlResolver;
        private readonly IDocsOptions _options;
        private readonly uint _baseHeadingLevel;

        public MarkdownTypeWriter(IMarkdownWriter writer, ISignatureFactory signatureFactory, IDocsUrlResolver urlResolver, IDocsOptions options)
        {
            _writer = writer;
            _signatureFactory = signatureFactory;
            _urlResolver = urlResolver;
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

            if (type.Category != TypeCategory.Delegate)
            {
                WriteMethods(type);
            }
        }

        private void WriteSummary(ITypeMetadata type)
        {

        }

        private void WriteSummary(IMethodMetadata method)
        {

        }

        private void WriteSummary(IParameterMetadata parameter)
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

        private void WriteSignature(IMethodMetadata method)
        {
            using (_writer.WriteCodeBlock())
            {
                string methodSignature = _signatureFactory.CreateMethod(method);
                _writer.Write(methodSignature);
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

        private void WriteMethods(ITypeMetadata type)
        {
            List<IMethodMetadata> methods = type.Methods.ToList();
            if (methods.Count > 0)
            {
                _writer.WriteHeading("Methods", _baseHeadingLevel + 1);

                foreach (IMethodMetadata method in methods)
                {
                    WriteSummary(method);
                    WriteSignature(method);
                    WriteParameters(method);

                    if (method.ReturnType.Name != typeof(void).Name)
                    {
                        _writer.WriteHeading("Returns", _baseHeadingLevel + 2);
                        string typeLink = method.ReturnType.Link(method.Owner, _urlResolver);
                        _writer.WriteLine(typeLink);
                    }
                }
            }
        }

        private void WriteParameters(IMethodMetadata method)
        {
            List<IParameterMetadata> parameters = method.Parameters.ToList();
            if (parameters.Count > 0)
            {
                _writer.WriteHeading("Parameters", _baseHeadingLevel + 2);

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
