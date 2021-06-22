using MarkdownDocs.Markdown;
using MarkdownDocs.Metadata;
using System;
using System.Linq;
using System.Text;

namespace MarkdownDocs
{
    public interface ISignatureFactory
    {
        string CreateParameter(IParameterMetadata parameter);
        string CreateConstructor(IConstructorMetadata constructor);
        string CreateMethod(IMethodMetadata method);
        string CreateDelegate(ITypeMetadata type);
        string CreateType(ITypeMetadata type);
        string CreateField(IFieldMetadata field);
        string CreateProperty(IPropertyMetadata property);
        string CreateEvent(IEventMetadata ev);
    }

    public class SignatureFactory : ISignatureFactory
    {
        private readonly IDocsOptions _options;
        private readonly IDocsUrlResolver _urlResolver;

        public SignatureFactory(IDocsOptions options, IDocsUrlResolver urlResolver)
        {
            _options = options;
            _urlResolver = urlResolver;
        }

        public string CreateConstructor(IConstructorMetadata constructor)
        {
            string parameters = string.Join(", ", constructor.Parameters.Select(CreateParameter));
            string result = $"{constructor.AccessModifier.ToMarkdown()} {constructor.Name}({parameters});".Clean();
            return result;
        }

        public string CreateField(IFieldMetadata field)
        {
            string assignValue = !string.IsNullOrWhiteSpace(field.RawValue) ? $" = {field.RawValue}" : string.Empty;
            string result = $"{field.AccessModifier.ToMarkdown()} {field.FieldModifier.ToMarkdown()} {_urlResolver.GetTypeName(field.Type, field.Owner, true)} {field.Name}{assignValue};".Clean();
            return result;
        }

        public string CreateProperty(IPropertyMetadata property)
        {
            string assignValue = !string.IsNullOrWhiteSpace(property.RawValue) ? $" = {property.RawValue};" : string.Empty;
            string getMethod = property.GetMethodModifier == AccessModifier.Protected && property.AccessModifier != AccessModifier.Protected ? "protected get;" : "get;";
            string setMethod = property.SetMethodModifier == AccessModifier.Protected && property.AccessModifier != AccessModifier.Protected ? "protected set;" : "set;";
            string result = $"{property.AccessModifier.ToMarkdown()} {property.PropertyModifier.ToMarkdown()} {_urlResolver.GetTypeName(property.Type, property.Owner, true)} {property.Name} {{ {getMethod} {setMethod} }}{assignValue}".Clean();
            return result;
        }

        public string CreateMethod(IMethodMetadata method)
        {
            string parameters = string.Join(", ", method.Parameters.Select(CreateParameter));
            string result = $"{method.AccessModifier.ToMarkdown()} {method.MethodModifier.ToMarkdown()} {_urlResolver.GetTypeName(method.ReturnType, method.Owner, true)} {method.Name}({parameters});".Clean();
            return result;
        }

        public string CreateParameter(IParameterMetadata parameter)
        {
            string assignValue = !string.IsNullOrWhiteSpace(parameter.RawValue) ? $" = {parameter.RawValue}" : string.Empty;
            string result = $"{_urlResolver.GetTypeName(parameter.Type, true)} {parameter.Name}{assignValue}".Clean();
            return result;
        }

        public string CreateEvent(IEventMetadata ev)
        {
            string result = $"{ev.AccessModifier.ToMarkdown()} {ev.EventModifier.ToMarkdown()} event {_urlResolver.GetTypeName(ev.Type, ev.Owner, true)} {ev.Name};".Clean();
            return result;
        }

        public string CreateDelegate(ITypeMetadata type)
        {
            if (type.Category != TypeCategory.Delegate)
            {
                throw new ArgumentException("Type must be a delegate.", nameof(type));
            }

            IMethodMetadata method = type.Methods.First(m => m.Name == nameof(Action.Invoke));
            string parameters = string.Join(", ", method.Parameters.Select(CreateParameter));
            string result = $"{method.AccessModifier.ToMarkdown()} {type.Category.ToMarkdown()} {_urlResolver.GetTypeName(method.ReturnType, type, true)} {type.Name}({parameters});".Clean();
            return result;
        }

        public string CreateType(ITypeMetadata type)
        {
            if (type.Category == TypeCategory.Delegate)
            {
                return CreateDelegate(type);
            }

            string title = $"{type.AccessModifier.ToMarkdown()} {type.Modifier.ToMarkdown()} {type.Category.ToMarkdown()} {type.Name}";
            StringBuilder builder = new StringBuilder(title);

            if (type.Category != TypeCategory.Enum)
            {
                string implemented = string.Join(", ", type.Implemented.Select(p => _urlResolver.GetTypeName(p, true)));
                bool inherits = type.Inherited != null && type.Inherited.Inherited != null;

                if (inherits || implemented.Length > 0)
                {
                    builder.Append(" : ");
                }

                if (inherits)
                {
                    builder.Append($"{_urlResolver.GetTypeName(type.Inherited!, true)}");
                }

                if (implemented.Length > 0)
                {
                    builder.Append($"{(inherits ? ", " : string.Empty)}{implemented}");
                }
            }

            string result = builder.ToString().Clean();
            return result;
        }
    }
}
