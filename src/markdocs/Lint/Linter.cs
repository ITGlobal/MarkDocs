using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Source;
using System;
using System.IO;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public static class Linter
    {
        public static void Run(string path, string tempDir, bool quiet, bool summary)
        {
            ICompilationReport report;
            
            using (var liveOutput = TerminalOutput.Create("running linter...", quiet))
            {
                var markdocs = MarkDocsFactory.Create(
                    config =>
                    {
                        config.UseEventListener(new LinterListener(liveOutput));

                        config.FromStaticDirectory(path);
                        config.UseCache(_ => new LinterCacheProvider());
                        config.UseResourceUrlResolver(new ResourceUrlResolver());
                        config.UseMarkdown(markdown =>
                        {
                            markdown.AddHighlightJs(tempDir);
                        });
                        config.UseLog(new SerilogLog());
                    }
                );
                using (markdocs)
                {
                    report = markdocs.Documentations[0].CompilationReport;
                }
            }

            Program.PrintReport(report, summary, quiet);
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