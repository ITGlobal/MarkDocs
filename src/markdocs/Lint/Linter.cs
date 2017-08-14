using System;
using System.IO;
using System.Linq;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Tools.Lint
{
    public static class Linter
    {
        public static void Run(
            string path,
            Func<Action<IServiceCollection>, ServiceProvider> factory,
            bool verbose,
            bool summary)
        {
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
                            config.UseCallback(new LinterCallback());
                        }

                        config.Storage.UseStaticDirectory(path);
                        config.Cache.Use(sp => sp.AddSingleton<ICache>(new LinterCache()));
                        config.Format.UseMarkdown(new MarkdownOptions
                        {
                            ResourceUrlResolver = new ResourceUrlResolver(),
                            SyntaxColorizer =
                                new ServerHighlightJsSyntaxColorizer(Path.Combine(Path.GetTempPath(),
                                    $"markdocs-lint-{Guid.NewGuid():N}"))
                        });
                    });
                });

                markdocs = provider.GetRequiredService<IMarkDocService>();
            }

            using (provider)
            {
                using (CliHelper.SpinnerSafe("Running linter..."))
                {
                    markdocs.Initialize();
                }

                var report = markdocs.Documentations[0].CompilationReport;
                Program.PrintReport(report, verbose);

                if (summary)
                {
                    var errors = report.Common
                                     .Count(_ => _.Type == CompilationReportMessageType.Error)
                                 + report.Pages.Select(p =>
                                         p.Messages.Count(_ => _.Type == CompilationReportMessageType.Error))
                                     .Sum();

                    var warnigs = report.Common
                                     .Count(_ => _.Type == CompilationReportMessageType.Warning)
                                 + report.Pages.Select(p =>
                                         p.Messages.Count(_ => _.Type == CompilationReportMessageType.Warning))
                                     .Sum();

                    Console.WriteLine("Summary");
                    Console.WriteLine("-------:");
                    Console.WriteLine($"  {errors} error(s)");
                    Console.WriteLine($"  {warnigs} warning(s)");
                }
            }
        }

        public static string GetRelativeResourcePath(IResource resource, IResource relativeTo)
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

        private static string GetResourcePath(IResource resource)
        {
            var path = resource.Id;
            if (path == "/")
            {
                path = "/index";
            }

            string extension;
            switch (resource.Type)
            {
                case ResourceType.Page:
                    extension = ".html";
                    break;
                default:
                    extension = Path.GetExtension(resource.FileName);
                    break;
            }

            path = Path.ChangeExtension(path, extension);
            return path;
        }
    }
}