using MarkdownDocs.Metadata;

namespace MarkdownDocs.Markdown
{
    public static class MarkdownFormat
    {
        public const string MicrosoftDocsUrl = "https://docs.microsoft.com/en-us/dotnet/api/";

        public static string Bold(this string text) => $"**{text.Trim()}**";

        public static string Link(this string text, string url) => $"[{text}]({url})";

        public static string Link(this ITypeMetadata type, ITypeMetadata root, bool displayNamespace = false)
        {
            string fullName = GetFullName(type);
            string result = displayNamespace ? fullName : type.Name;

            if (type.IsMicrosoftType)
            {
                return result.Link($"{MicrosoftDocsUrl}{fullName}");
            }

            string[] path = fullName.Split(".");
            string link = string.Join("/", path);

            if (displayNamespace && type.Namespace != root.Namespace)
            {
                return fullName.Link(link);
            }

            return type.Name.Link(link);

            static string GetFullName(ITypeMetadata type) => $"{type.Namespace}.{type.Name}";
        }
    }
}
