using System.IO;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class MarkdownWriter : IMarkdownWriter
    {
        public static readonly string LineBreak = $"{new string(' ', 2)}\r\n";

        private readonly StreamWriter _stream;

        public MarkdownWriter(StreamWriter stream) => _stream = stream;

        public ValueTask DisposeAsync() => _stream.DisposeAsync();

        public void Write(string text) => _stream.Write(text);

        public void WriteLine(string? text = null) => _stream.WriteLine($"{text}{LineBreak}");

        public void WriteHeading(string text, uint level = 1)
        {
            string indent = new string('#', (int)level);
            WriteLine($"{indent} {text}");
        }
    }
}
