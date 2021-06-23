using MarkdownDocs.Context;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MarkdownDocs.Resolver
{
    public class XMLResolver : IDocResolver
    {
        private readonly IDocsOptions _options;
        private readonly IXMLMemberResolver _memberResolver;

        public XMLResolver(IDocsOptions options, IXMLMemberResolver memberResolver)
        {
            _options = options;
            _memberResolver = memberResolver;
        }

        public async Task ResolveAsync(IAssemblyContext context, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string xmlFilePath = Path.ChangeExtension(_options.InputPath, "xml");

            using var stream = new StreamReader(xmlFilePath);

            XElement elem = await XElement.LoadAsync(stream, LoadOptions.None, cancellationToken).ConfigureAwait(false);
            XElement members = elem.Element("members")!;

            foreach (XElement member in members.Elements())
            {
                await _memberResolver.ResolveAsync(context, member, cancellationToken);
            }
        }
    }
}
