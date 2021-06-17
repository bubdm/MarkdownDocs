using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;

namespace MarkdownDocs
{
    public interface IDocsUrlResolver
    {
        string ResolveUrl(ITypeMetadata type);
        string GetTypeName(ITypeMetadata type, bool pretty = false);
        string GetTypeName(ITypeMetadata type, ITypeMetadata root, bool pretty = false);
    }

    public class DocsUrlResolver : IDocsUrlResolver
    {
        private readonly IDocsOptions _options;

        public const string Microsoft = "Microsoft Corporation";
        public const string MicrosoftDocsUrl = "https://docs.microsoft.com/en-us/dotnet/api/";

        private static readonly Dictionary<string, string> _typeNameMap = new Dictionary<string, string>
        {
            [typeof(float).Name] = "float",
            [typeof(int).Name] = "int",
            [typeof(bool).Name] = "bool",
            [typeof(string).Name] = "string",
            [typeof(double).Name] = "double",
            [typeof(decimal).Name] = "decimal",
            [typeof(char).Name] = "char",
            [typeof(object).Name] = "object",
            [typeof(void).Name] = "void"
        };

        public DocsUrlResolver(IDocsOptions options)
        {
            _options = options;
        }

        public string ResolveUrl(ITypeMetadata type)
        {
            string baseUrl = string.Empty; // TODO: #1 use value from options

            if (!string.IsNullOrWhiteSpace(type.Company))
            {
                if (type.Company.Contains(Microsoft, StringComparison.OrdinalIgnoreCase))
                {
                    return $"{MicrosoftDocsUrl}{type.FullName}";
                }
            }

            if (_options.IsCompact)
            {
                return $"#{type.Name}-{type.Category}".ToLowerInvariant();
            }

            string[] path = type.FullName.Split(".");
            string link = string.Join("/", path);
            link = string.IsNullOrWhiteSpace(baseUrl) ? link : $"{baseUrl}/{link}";
            return link;
        }

        public string GetTypeName(ITypeMetadata type, bool pretty = false)
        {
            if (pretty && _typeNameMap.TryGetValue(type.Name, out string? prettyName))
            {
                return prettyName;
            }

            bool displayNamespace = false; // TODO: #1 use value from options
            string result = displayNamespace ? type.FullName : type.Name;
            return result;
        }

        public string GetTypeName(ITypeMetadata type, ITypeMetadata relativeTo, bool pretty = false)
        {
            if (pretty && _typeNameMap.TryGetValue(type.Name, out string? prettyName))
            {
                return prettyName;
            }

            bool displayNamespace = false; // TODO: #1 use value from options
            if (displayNamespace && type.Namespace != relativeTo.Namespace)
            {
                return type.FullName;
            }

            return GetTypeName(type, pretty);
        }
    }
}
