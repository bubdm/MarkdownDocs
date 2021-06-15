using System.IO;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class MarkdownWriter : IMarkdownWriter
    {
        private readonly StreamWriter _stream;

        public MarkdownWriter(StreamWriter stream) => _stream = stream;

        public ValueTask DisposeAsync()
        {
            return _stream.DisposeAsync();
        }
    }
}
