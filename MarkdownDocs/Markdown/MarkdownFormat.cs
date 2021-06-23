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

        public static string Sanitize(this string text)
        {
            int genericStart = text.IndexOf('<');
            int genericEnd = text.IndexOf('>');

            if (genericStart != genericEnd)
            {
                return text.Remove(genericStart, genericEnd - genericStart + 1);
            }

            return text;
        }

        public static string Link(this ITypeMetadata type, in ITypeMetadata relativeTo, in IDocsUrlResolver resolver)
        {
            string name = resolver.GetTypeName(type, relativeTo);
            if (!string.IsNullOrWhiteSpace(type.Assembly))
            {
                string url = resolver.ResolveUrl(type);
                return name.Link(url);
            }

            return name;
        }

        public static string Link(this IMemberMetadata member, in ITypeMetadata relativeTo, in IDocsUrlResolver resolver)
        {
            ITypeMetadata type = member.Owner;
            string name = resolver.GetTypeName(type, relativeTo);
            string memberName = $"{name}.{member.Name}";
            if (!string.IsNullOrWhiteSpace(type.Assembly))
            {
                string url = resolver.ResolveUrl(member);
                return memberName.Link(url);
            }

            return memberName;
        }

        public static string Link(this string memberName, in ITypeMetadata type, in ITypeMetadata relativeTo, in IDocsUrlResolver resolver)
        {
            string name = resolver.GetTypeName(type, relativeTo);
            string result = $"{name}.{memberName}";
            if (!string.IsNullOrWhiteSpace(type.Assembly))
            {
                string url = resolver.ResolveUrl(memberName, type);
                return result.Link(url);
            }

            return result;
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

        public static string ToMarkdown(this MethodModifier modifier)
            => modifier switch
            {
                MethodModifier.None => string.Empty,
                MethodModifier.Override => "override",
                MethodModifier.Virtual => "virtual",
                MethodModifier.Abstract => "abstract",
                MethodModifier.Static => "static",
                _ => throw new NotImplementedException(),
            };

        public static string ToMarkdown(this FieldModifier modifier)
            => modifier switch
            {
                FieldModifier.None => string.Empty,
                FieldModifier.Static => "static",
                FieldModifier.Const => "const",
                FieldModifier.Readonly => "readonly",
                _ => throw new NotImplementedException(),
            };

        public static string ToMarkdown(this TypeCategory cateogry)
            => cateogry.ToString().ToLowerInvariant();
    }
}
