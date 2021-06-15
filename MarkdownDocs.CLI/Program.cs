using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MarkdownDocs.CLI
{
    class Program
    {
        async static Task Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintHelp();
                return;
            }

            // Consider using https://github.com/commandlineparser/commandline
            var result = new OptionsBuilder(args).Build();
            if (result.IsValid)
            {
                try
                {
                    // Consider adding a --timeout option to cancel token
                    await MarkdownCLI.New(result.Options!).WriteDocsAsync(CancellationToken.None).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Writing was cancelled.");
                }
            }
            else
            {
                PrintErrors(result.Errors);
                PrintHelp();
            }
        }

        public static void PrintHelp()
        {
            Console.WriteLine("markdown input -o output [--compact] [--noxml] [--parallel-writes]");
            Console.WriteLine();
        }

        public static void PrintErrors(IEnumerable<string> errors)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
            Console.ForegroundColor = originalColor;
            Console.WriteLine();
        }
    }
}
