using System;
using System.IO;
using ITGlobal.CommandLine;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Storage;
using Microsoft.Extensions.Logging;
using Serilog;
using static ITGlobal.CommandLine.Terminal;

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
                    Stdout.WriteLine(
                        $"Unable to find template {templateName}".WithForeground(ConsoleColor.Red)
                    );
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

            Stdout.WriteLine();
            Stdout.Write("Generating static website from ");
            Stdout.Write(sourceDir.WithForeground(ConsoleColor.Cyan));
            Stdout.Write(" into ");
            Stdout.Write(targetDir.WithForeground(ConsoleColor.Cyan));
            Stdout.Write(" using ");
            Stdout.Write(template.Name.WithForeground(ConsoleColor.Cyan));
            Stdout.WriteLine(" template");

            using (markdocs)
            {
                markdocs.Initialize();

                Program.PrintReport(markdocs.Documentations[0].CompilationReport, verbose);
            }

            return 0;
        }
    }
}
