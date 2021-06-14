using System;
using System.Collections.Generic;
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

            var result = new OptionsBuilder(args).Build();
            if (result.IsValid)
            {
                await MarkdownCLI.New(result.Options!).WriteDocsAsync();
            }
            else
            {
                PrintErrors(result.Errors);
                PrintHelp();
            }
        }

        public static void PrintHelp()
        {
            Console.WriteLine("markdown input -o output [--compact] [--noxml]");
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
