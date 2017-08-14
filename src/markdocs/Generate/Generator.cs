using System;
using System.IO;
using ITGlobal.CommandLine;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public static class Generator
    {
        public static int Run(
            Func<Action<IServiceCollection>, ServiceProvider> factory,
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

            ServiceProvider provider;
            IMarkDocService markdocs;
            
            using (CliHelper.SpinnerSafe("Initializing..."))
            {
                provider = factory(services =>
                {
                    services.AddMarkDocs(config =>
                    {
                        if (verbose)
                        {
                            config.UseCallback(new GeneratorCallback());
                        }

                        config.Storage.UseStaticDirectory(sourceDir);
                        config.Cache.Use(sp => sp.AddSingleton<ICache>(new OutputCache(targetDir, template)));
                        config.Format.UseMarkdown(new MarkdownOptions
                        {
                            ResourceUrlResolver = new GeneratorResourceUrlResolver(),
                            SyntaxColorizer =
                                new ServerHighlightJsSyntaxColorizer(Path.Combine(Path.GetTempPath(),
                                    $"markdocs-build-{Guid.NewGuid():N}"))
                        });
                    });
                });

                markdocs = provider.GetRequiredService<IMarkDocService>();
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

            using (provider)
            {
                
                markdocs.Initialize();

                Program.PrintReport(markdocs.Documentations[0].CompilationReport, verbose);
            }

            return 0;
        }
    }
}
