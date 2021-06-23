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
        private readonly IMetadataWriter<IConstructorMetadata> _constructorWriter;
        private readonly IMetadataWriter<IFieldMetadata> _fieldWriter;
        private readonly IMetadataWriter<IPropertyMetadata> _propertyWriter;
        private readonly IMetadataWriter<IEventMetadata> _eventWriter;
        private readonly IMetadataWriter<IParameterMetadata> _parameterWriter;

        public TypeMetaWriter(IMarkdownWriter writer,
            ISignatureFactory signatureFactory,
            IDocsUrlResolver urlResolver,
            Func<IMarkdownWriter, IMetadataWriter<IMethodMetadata>> methodWriterFactory,
            Func<IMarkdownWriter, IMetadataWriter<IConstructorMetadata>> constructorWriterFactory,
            Func<IMarkdownWriter, IMetadataWriter<IFieldMetadata>> fieldWriterFactory,
            Func<IMarkdownWriter, IMetadataWriter<IPropertyMetadata>> propertyWriterFactory,
            Func<IMarkdownWriter, IMetadataWriter<IEventMetadata>> eventWriterFactory,
            Func<IMarkdownWriter, IMetadataWriter<IParameterMetadata>> parameterWriterFactory)
        {
            _writer = writer;
            _signatureFactory = signatureFactory;
            _urlResolver = urlResolver;
            _methodWriter = methodWriterFactory(writer);
            _constructorWriter = constructorWriterFactory(writer);
            _fieldWriter = fieldWriterFactory(writer);
            _propertyWriter = propertyWriterFactory(writer);
            _eventWriter = eventWriterFactory(writer);
            _parameterWriter = parameterWriterFactory(writer);
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

            await WriteConstructorsAsync(type, indent + 1, cancellationToken).ConfigureAwait(false);
            await WriteFieldsAsync(type, indent + 1, cancellationToken).ConfigureAwait(false);
            await WritePropertiesAsync(type, indent + 1, cancellationToken).ConfigureAwait(false);

            if (type.Category != TypeCategory.Delegate)
            {
                await WriteMethodsAsync(type, indent + 1, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await WriteDelegateAsync(type, indent, cancellationToken).ConfigureAwait(false);
            }

            await WriteEventsAsync(type, indent + 1, cancellationToken).ConfigureAwait(false);
        }

        private async Task WriteDelegateAsync(ITypeMetadata type, uint indent, CancellationToken cancellationToken)
        {
            var invokeMethod = type.Methods.First(m => m.Name == "Invoke");
            await WriteParametersAsync(invokeMethod, indent, cancellationToken).ConfigureAwait(false);

            if (invokeMethod.ReturnType.Name != typeof(void).Name)
            {
                _writer.WriteLine("Returns".Bold());
                string typeLink = invokeMethod.ReturnType.Link(invokeMethod.Owner, _urlResolver);
                _writer.WriteLine(typeLink);
            }
        }

        private async Task WriteParametersAsync(IMethodMetadata method, uint indent, CancellationToken cancellationToken)
        {
            List<IParameterMetadata> parameters = method.Parameters.ToList();
            if (parameters.Count > 0)
            {
                _writer.WriteLine("Parameters".Bold());

                foreach (IParameterMetadata parameter in parameters)
                {
                    await _parameterWriter.WriteAsync(parameter, indent, cancellationToken).ConfigureAwait(false);
                }
            }
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
                    await _methodWriter.WriteAsync(method, indent + 1, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task WriteConstructorsAsync(ITypeMetadata type, uint indent, CancellationToken cancellationToken)
        {
            List<IConstructorMetadata> constructors = type.Constructors.ToList();
            if (constructors.Count > 0)
            {
                _writer.WriteHeading("Constructors", indent);

                foreach (IConstructorMetadata constructor in constructors)
                {
                    await _constructorWriter.WriteAsync(constructor, indent + 1, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task WriteFieldsAsync(ITypeMetadata type, uint indent, CancellationToken cancellationToken)
        {
            List<IFieldMetadata> fields = type.Fields.OrderBy(f => f.Name).ToList();
            if (fields.Count > 0)
            {
                _writer.WriteHeading("Fields", indent);

                foreach (IFieldMetadata field in fields)
                {
                    await _fieldWriter.WriteAsync(field, indent + 1, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task WritePropertiesAsync(ITypeMetadata type, uint indent, CancellationToken cancellationToken)
        {
            List<IPropertyMetadata> properties = type.Properties.OrderBy(f => f.Name).ToList();
            if (properties.Count > 0)
            {
                _writer.WriteHeading("Properties", indent);

                foreach (IPropertyMetadata property in properties)
                {
                    await _propertyWriter.WriteAsync(property, indent + 1, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task WriteEventsAsync(ITypeMetadata type, uint indent, CancellationToken cancellationToken)
        {
            List<IEventMetadata> events = type.Events.OrderBy(f => f.Name).ToList();
            if (events.Count > 0)
            {
                _writer.WriteHeading("Events", indent);

                foreach (IEventMetadata ev in events)
                {
                    await _eventWriter.WriteAsync(ev, indent + 1, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private void WriteSummary(ITypeMetadata type)
        {
            if (!string.IsNullOrWhiteSpace(type.Description))
            {
                _writer.WriteLine(type.Description);
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
