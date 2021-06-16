using System;
using System.IO;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class MarkdownWriter : IMarkdownWriter
    {
        public static readonly string LineBreak = $"{new string(' ', 2)}\r\n";

        private readonly StreamWriter _stream;
        private bool _isWritingCode;

        public MarkdownWriter(StreamWriter stream)
        {
            _stream = stream;
            _stream.NewLine = LineBreak;
        }

        public ValueTask DisposeAsync() => _stream.DisposeAsync();

        public void Write(string text) => _stream.Write(text);

        public void WriteLine(string? text = null) => _stream.WriteLine(_isWritingCode ? text : $"{text}{LineBreak}");

        public void WriteHeading(string text, uint level = 1)
        {
            string indent = new string('#', (int)level);
            WriteLine($"{indent} {text}");
        }

        public IDisposable WriteCode(bool multiline) => new CodeBlockWriter(this, multiline);

        private class CodeBlockWriter : IDisposable
        {
            private readonly MarkdownWriter _writer;
            private readonly bool _multiline;

            public CodeBlockWriter(MarkdownWriter writer, bool multiline)
            {
                _writer = writer;
                _multiline = multiline;
                _writer._isWritingCode = true;

                if (_multiline)
                {
                    _writer.WriteLine("```csharp");
                }
                else
                {
                    _writer.Write("`");
                }
            }

            public void Dispose()
            {
                if (_multiline)
                {
                    _writer.WriteLine();
                    _writer.WriteLine("```");
                }
                else
                {
                    _writer.Write("`");
                }

                _writer._isWritingCode = false;
            }
        }
    }
}
