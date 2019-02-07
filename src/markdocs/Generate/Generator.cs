using ITGlobal.CommandLine;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Source;
using System;
using System.IO;
using System.Text;
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

            Stdout.WriteLine();
            Stdout.Write("Generating static website from ");
            Stdout.Write(sourceDir.WithForeground(ConsoleColor.Cyan));
            Stdout.Write(" into ");
            Stdout.Write(targetDir.WithForeground(ConsoleColor.Cyan));
            Stdout.Write(" using ");
            Stdout.Write(template.Name.WithForeground(ConsoleColor.Cyan));
            Stdout.WriteLine(" template");

            ICompilationReport report;
            using (CliHelper.SpinnerSafe("Running..."))
            {
                var markdocs = MarkDocsFactory.Create(
                    config =>
                    {
                        if (verbose)
                        {
                            config.UseCallback(new GeneratorCallback());
                        }

                        config.FromStaticDirectory(sourceDir);
                        config.UseCache(_ => new OutputCache(targetDir, template));
                        config.UseResourceUrlResolver(_ => new GeneratorResourceUrlResolver());
                        config.UseMarkdown(markdown =>
                        {
                            markdown.CodeBlocks.UseServerSideHighlightJs(
                                Path.Combine(Path.GetTempPath(), $"markdocs-build-{Guid.NewGuid():N}")
                            );
                        });
                        config.UseLog(new SerilogLog());
                    }
                );

                var documentation = markdocs.Documentations[0];
                report = documentation.CompilationReport;

                RenderPage(documentation.RootPage);

                void RenderPage(IPage page)
                {
                    var path = Path.Combine(targetDir, page.RelativePath != "/" ? page.Id.Substring(1) : "index");
                    path = Path.ChangeExtension(path, ".html");

                    var content = page.ReadHtmlString();
                    using (var writer = new StreamWriter(path, false, Encoding.UTF8))
                    {
                        template.Render(page, content, writer);
                    }

                    foreach (var nestedPage in page.NestedPages)
                    {
                        RenderPage(nestedPage);
                    }
                }

            }

            Program.PrintReport(report, verbose);

            return 0;
        }
    }
}
