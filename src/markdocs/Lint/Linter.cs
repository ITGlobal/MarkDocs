using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Source;
using System;
using System.IO;
using System.Linq;
using static ITGlobal.CommandLine.Terminal;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public static class Linter
    {
        public static void Run(
            string path,
            bool verbose,
            bool summary)
        {
            ICompilationReport report;
            using (CliHelper.SpinnerSafe("Running linter..."))
            {
                var markdocs = MarkDocsFactory.Create(
                    config =>
                    {
                        if (verbose)
                        {
                            config.UseCallback(new LinterCallback());
                        }

                        config.FromStaticDirectory(path);
                        config.UseCache(_ => new LinterCacheProvider());
                        config.UseResourceUrlResolver(new ResourceUrlResolver());
                        config.UseMarkdown(markdown =>
                        {
                            markdown.CodeBlocks.UseServerSideHighlightJs(
                                Path.Combine(Path.GetTempPath(), $"markdocs-lint-{Guid.NewGuid():N}")
                            );
                        });
                        config.UseLog(new SerilogLog());
                    }
                );
                using (markdocs)
                {
                    report = markdocs.Documentations[0].CompilationReport;
                }
            }

            Program.PrintReport(report, verbose);

            if (summary)
            {
                var errors = report.Messages.Values.Sum(_ => _.Count(x => x.Type == CompilationReportMessageType.Error)); ;
                var warnings = report.Messages.Values.Sum(_ => _.Count(x => x.Type == CompilationReportMessageType.Warning));

                Stdout.WriteLine("Summary");
                Stdout.WriteLine("-------");
                Stdout.WriteLine($"  {errors} error(s)");
                Stdout.WriteLine($"  {warnings} warning(s)");
            }
        }

        public static string GetRelativeResourcePath(IResourceId resource, IResourceId relativeTo)
        {
            var relativeToUrl = new Uri($"http://site{GetResourcePath(relativeTo)}");
            var resourceUrl = new Uri($"http://site{GetResourcePath(resource)}");

            var url = relativeToUrl.MakeRelativeUri(resourceUrl);
            var urlstr = url.ToString();

            if (urlstr.StartsWith("/"))
            {
                urlstr = urlstr.Substring(1);
            }

            if (string.IsNullOrEmpty(urlstr))
            {
                urlstr = Path.GetFileName(relativeToUrl.LocalPath);
            }

            return urlstr;
        }

        private static string GetResourcePath(IResourceId resource)
        {
            var path = resource.Id;
            if (path == "/")
            {
                path = "/index";
            }

            if (resource.Type == ResourceType.Page)
            {
                path = Path.ChangeExtension(path, ".html");
            }

            return path;
        }
    }
}