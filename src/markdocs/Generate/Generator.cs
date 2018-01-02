using System;
using System.IO;
using ITGlobal.CommandLine;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Storage;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public static class Generator
    {
        public static int Run(
            string sourceDir,
            string targetDir,
            string templateName,
            bool verbose)
        {
            ITemplate template;
            switch ((templateName ?? "").ToLowerInvariant())
            {
                case "":
                case "preview":
                    template = new PreviewTemplate();
                    break;
                case "minimal":
                    template = new MinimalBootstrapTemplate();
                    break;

                default:
                    using (CLI.WithForeground(ConsoleColor.Red))
                    {
                        Console.WriteLine($"Unable to find template {templateName}");
                    }
                    return 1;
            }
            
            IMarkDocService markdocs;
            
            using (CliHelper.SpinnerSafe("Initializing..."))
            {
                markdocs = MarkDocsFactory.Create(
                    config =>
                    {
                        if (verbose)
                        {
                            config.UseCallback(new GeneratorCallback());
                        }

                        config.Storage.UseStaticDirectory(sourceDir);
                        config.Cache.Use(_ => new OutputCache(targetDir, template));
                        config.Format.UseMarkdown(new MarkdownOptions
                        {
                            ResourceUrlResolver = new GeneratorResourceUrlResolver(),
                            SyntaxColorizer =
                                new ServerHighlightJsSyntaxColorizer(Path.Combine(Path.GetTempPath(),
                                    $"markdocs-build-{Guid.NewGuid():N}"))
                        });
                    },
                    new LoggerFactory().AddSerilog()
                );
            }

            Console.WriteLine();
            Console.Write("Generating static website from ");
            using (CLI.WithForeground(ConsoleColor.Cyan))
            {
                Console.Write(sourceDir);
            }
            Console.Write(" into ");
            using (CLI.WithForeground(ConsoleColor.Cyan))
            {
                Console.Write(targetDir);
            }
            Console.Write(" using ");
            using (CLI.WithForeground(ConsoleColor.Cyan))
            {
                Console.Write(template.Name);
            }
            Console.WriteLine(" template");

            using (markdocs)
            {
                markdocs.Initialize();

                Program.PrintReport(markdocs.Documentations[0].CompilationReport, verbose);
            }

            return 0;
        }
    }
}
