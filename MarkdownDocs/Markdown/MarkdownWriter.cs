using System;
using System.IO;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class MarkdownWriter : IMarkdownWriter
    {
        public static readonly string LineBreak = $"{new string(' ', 2)}\r\n";

        private readonly StreamWriter _stream;
        private bool _isWritingCodeBlock;

        public MarkdownWriter(StreamWriter stream)
        {
            _stream = stream;
            _stream.NewLine = LineBreak;
        }

        public ValueTask DisposeAsync() => _stream.DisposeAsync();

        public void Write(string text) => _stream.Write(text);

        public void WriteLine(string? text = null) => _stream.WriteLine(_isWritingCodeBlock ? text : $"{text}{LineBreak}");

        public void WriteHeading(string text, uint level = 1)
        {
            string indent = new string('#', (int)level);
            WriteLine($"{indent} {text}");
        }

        public IDisposable WriteCodeBlock() => new CodeBlockWriter(this);

        public void WriteCode(string? text) => Write($"`{text}`");

        private class CodeBlockWriter : IDisposable
        {
            private readonly MarkdownWriter _writer;

            public CodeBlockWriter(MarkdownWriter writer)
            {
                _writer = writer;
                _writer._isWritingCodeBlock = true;

                _writer.WriteLine("```csharp");
            }

            public void Dispose()
            {
                _writer.WriteLine();
                _writer.WriteLine("```");

                _writer._isWritingCodeBlock = false;
            }
        }
    }
}
