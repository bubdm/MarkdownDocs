using System;

namespace MarkdownDocs.Markdown
{
    public interface IMarkdownWriter : IAsyncDisposable
    {
        void WriteHeading(string text, uint level = 1);
        IDisposable WriteCode(bool multiline = true);
        void WriteLine(string? text = default);
        void Write(string text);
    }
}
