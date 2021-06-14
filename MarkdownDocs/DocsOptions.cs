namespace MarkdownDocs
{
    public class DocsOptions
    {
        public bool IsCompact { get; }
        public bool UseXML { get; }

        public DocsOptions(bool isCompact, bool useXML)
        {
            IsCompact = isCompact;
            UseXML = useXML;
        }
    }
}
