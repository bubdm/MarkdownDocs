using MarkdownDocs.Metadata;
using System;
using System.Text.RegularExpressions;

namespace MarkdownDocs.Markdown
{
    public static class MarkdownFormat
    {
        public static string Bold(this string text) => $"**{text.Trim()}**";

        public static string Link(this string text, in string url) => $"[{text}]({url})";

        public static string Clean(this string text) => new Regex("[ ]{2,}", RegexOptions.None).Replace(text, " ");

        public static string Link(this ITypeMetadata type, in ITypeMetadata relativeTo, in IDocsUrlResolver resolver)
        {
            string name = resolver.GetTypeName(type, relativeTo);
            string url = resolver.ResolveUrl(type);
            return name.Link(url);
        }

        public static string ToMarkdown(this AccessModifier modifier)
            => modifier switch
            {
                AccessModifier.Public => "public",
                AccessModifier.Protected => "protected",
                _ => throw new NotImplementedException(),
            };

        public static string ToMarkdown(this TypeModifier modifier)
            => modifier switch
            {
                TypeModifier.None => string.Empty,
                TypeModifier.Abstract => "abstract",
                TypeModifier.Sealed => "sealed",
                TypeModifier.Static => "static",
                _ => throw new NotImplementedException(),
            };

        public static string ToMarkdown(this TypeCategory cateogry)
            => cateogry.ToString().ToLowerInvariant();
    }
}
