using MarkdownDocs.Metadata;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public static class TypeWriter
    {
        public static async Task WriteAsync(this ITypeMetadata type, IMarkdownWriter writer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            writer.WriteHeading($"{type.Name} {type.Category}");
            writer.WriteLine($"{"Namespace:".Bold()} {type.Namespace}");
            //writer.WriteLine($"{"Assembly:".Bold()} {type.Assembly}");

            if (type.Category == TypeCategory.Class)
            {
                writer.WriteLine($"{"Inheritance:".Bold()} {GetInheritanceChain(type)}");
            }
        }

        private static string GetInheritanceChain(ITypeMetadata type)
        {
            var baseTypes = new List<string>();
            ITypeMetadata? current = type.Inherited;

            while (current != null)
            {
                baseTypes.Add(current.Link(type));
                current = current.Inherited;
            }

            baseTypes.Reverse();
            string result = string.Join(" → ", baseTypes);
            return result;
        }

        public static async Task WriteAsync(this PropertyMetadata prop, IMarkdownWriter writer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

        }

        public static async Task WriteAsync(this MethodMetadata method, IMarkdownWriter writer, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

        }
    }
}
