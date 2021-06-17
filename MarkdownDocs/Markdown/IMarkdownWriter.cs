using System;

namespace MarkdownDocs.Markdown
{
    public interface IMarkdownWriter : IAsyncDisposable
    {
        void WriteHeading(string text, uint level = 1);
        IDisposable WriteCodeBlock();
        void WriteCode(string? text);
        void WriteLine(string? text = default);
        void Write(string text);
    }
}
