using MarkdownDocs.Metadata;
using System.Text.RegularExpressions;

namespace MarkdownDocs.Markdown
{
    public static class MarkdownFormat
    {
        public const string MicrosoftDocsUrl = "https://docs.microsoft.com/en-us/dotnet/api/";

        public static string Bold(this string text) => $"**{text.Trim()}**";

        public static string Link(this string text, in string url) => $"[{text}]({url})";

        public static string Clean(this string text) => new Regex("[ ]{2,}", RegexOptions.None).Replace(text, " ");

        public static string Link(this ITypeMetadata type, in ITypeMetadata root, in IDocsOptions options)
        {
            string baseUrl = string.Empty; // TODO: #1 use value from options
            string name = type.GetName(options);
            string fullName = GetFullName(type);

            if (type.IsMicrosoftType)
            {
                return name.Link($"{MicrosoftDocsUrl}{fullName}");
            }

            if (options.IsCompact)
            {
                return name.Link($"#{type.Name}-{type.Category}".ToLowerInvariant());
            }

            string[] path = fullName.Split(".");
            string link = string.Join("/", path);
            link = string.IsNullOrWhiteSpace(baseUrl) ? link : $"{baseUrl}/{link}";

            return type.Namespace != root.Namespace ? fullName.Link(link) : type.Name.Link(link);
        }

        public static string GetName(this ITypeMetadata type, IDocsOptions options)
        {
            bool displayNamespace = false; // TODO: #1 use value from options
            string result = displayNamespace ? GetFullName(type) : type.Name;
            return result;
        }

        static string GetFullName(ITypeMetadata type) => $"{type.Namespace}.{type.Name}";

        public static string ToMarkdown(this AccessModifier modifier)
            => modifier switch
            {
                AccessModifier.Public => "public",
                AccessModifier.Protected => "protected",
                _ => throw new System.NotImplementedException(),
            };

        public static string ToMarkdown(this TypeModifier modifier)
            => modifier switch
            {
                TypeModifier.None => string.Empty,
                TypeModifier.Abstract => "public",
                TypeModifier.Sealed => "sealed",
                TypeModifier.Static => "static",
                _ => throw new System.NotImplementedException(),
            };

        public static string ToMarkdown(this TypeCategory cateogry)
            => cateogry.ToString().ToLowerInvariant();
    }
}
