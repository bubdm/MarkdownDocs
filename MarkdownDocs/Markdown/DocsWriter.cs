using MarkdownDocs.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.Markdown
{
    public class DocsWriter : IDocsWriter
    {
        private readonly Func<StreamWriter, IMarkdownWriter> _mdWriterFactory;
        private readonly Func<IMarkdownWriter, IMetadataWriter<ITypeMetadata>> _typeWriterFactory;

        public DocsWriter(Func<StreamWriter, IMarkdownWriter> mdWriterFactory, Func<IMarkdownWriter, IMetadataWriter<ITypeMetadata>> typeWriterFactory)
        {
            _mdWriterFactory = mdWriterFactory;
            _typeWriterFactory = typeWriterFactory;
        }

        public async Task WriteAsync(IAssemblyMetadata assembly, IDocsOptions options, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            IEnumerable<ITypeMetadata> exportedTypes = assembly.Types.Where(t => t.Assembly == assembly.Name).OrderBy(t => t.Name);

            uint indent = options.IsCompact ? 2u : 1u;

            if (options.IsCompact)
            {
                string outputFolder = Path.GetDirectoryName(options.OutputPath) ?? Directory.GetCurrentDirectory();
                Directory.CreateDirectory(outputFolder);

                var stream = new StreamWriter(options.OutputPath);
                await using (stream.ConfigureAwait(false))
                {
                    IMarkdownWriter writer = _mdWriterFactory(stream);
                    await using (writer.ConfigureAwait(false))
                    {
                        IMetadataWriter<ITypeMetadata> typeWriter = _typeWriterFactory(writer);
                        foreach (ITypeMetadata type in exportedTypes)
                        {
                            await typeWriter.WriteAsync(type, indent, cancellationToken).ConfigureAwait(false);
                        }
                    }
                }
            }
            else
            {
                if (options.ParallelWrites)
                {
                    IEnumerable<Task> tasks = exportedTypes.Select(type => WriteTypeToFileAsync(options.OutputPath, type, indent, cancellationToken));
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                else
                {
                    foreach (ITypeMetadata type in exportedTypes)
                    {
                        await WriteTypeToFileAsync(options.OutputPath, type, indent, cancellationToken).ConfigureAwait(false);
                    }
                }
            }
        }

        private async Task WriteTypeToFileAsync(string outputPath, ITypeMetadata type, uint indent, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string[] folders = type.Namespace?.Split(".", StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
            string outputFileName = $"{type.Name}.md";
            string outputFolder = Path.Join(outputPath, Path.Join(folders));
            string outputFilePath = Path.Join(outputFolder, outputFileName);

            Directory.CreateDirectory(outputFolder);

            var stream = new StreamWriter(outputFilePath);
            await using (stream.ConfigureAwait(false))
            {
                IMarkdownWriter writer = _mdWriterFactory(stream);
                await using (writer.ConfigureAwait(false))
                {
                    await _typeWriterFactory(writer).WriteAsync(type, indent, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
