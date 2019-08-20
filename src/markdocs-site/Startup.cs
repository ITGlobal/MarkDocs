using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Search;
using ITGlobal.MarkDocs.Site.Extensions;
using ITGlobal.MarkDocs.Site.Middlewares;
using ITGlobal.MarkDocs.Source;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using static ITGlobal.MarkDocs.Site.Env;

namespace ITGlobal.MarkDocs.Site
{
    public class Startup
    {
        public static IServiceProvider ApplicationServices { get; private set; }

        private readonly Stopwatch _startupTimer = Stopwatch.StartNew();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMarkDocs(ConfigureMarkDocs);
            services.AddResponseCaching();
            services.AddResponseCompression();
        }

        private void ConfigureMarkDocs(MarkDocsOptions config)
        {
            Directory.CreateDirectory(TEMP_DIR);

            config.UseDiskCache(Path.Combine(TEMP_DIR, "cache"));

            config.UseMarkdown(markdown =>
                {
                    markdown.AddHighlightJs(Path.Combine(TEMP_DIR, "hljs"));
                    markdown.AddCodecogsMathRenderer();
                    markdown.AddPlantUmlWebService();
                    markdown.UseTocRenderer<CustomTocRenderer>();
                    markdown.OverrideRendering<LinkInline>(ImageRenderer.RenderImage);
                    markdown.OverrideRendering<QuoteBlock>(QuoteBlockRenderer.Render);
                });
            config.UseResourceUrlResolver(new ResourceUrlResolver());
            config.UseAspNetLog();

            if (!string.IsNullOrEmpty(DOC_SOURCE_DIR))
            {
                config.FromStaticDirectory(DOC_SOURCE_DIR, watchForChanges: true);
            }
            else if (!string.IsNullOrEmpty(DOC_GIT_FETCH_URL))
            {
                var storageOptions = new GitSourceTreeOptions
                {
                    Directory = Path.Combine(TEMP_DIR, "src"),
                    FetchUrl = DOC_GIT_FETCH_URL,
                    UserName = DOC_GIT_USERNAME,
                    Password = DOC_GIT_PASSWORD,
                    EnablePolling = true,
                    PollingInterval = TimeSpan.FromSeconds(DOC_GIT_POLL_INTERVAL),
                    Branches =
                    {
                        Include = DOC_GIT_BRANCH
                    }
                };
                config.FromGit(storageOptions);
            }
            else
            {
                throw new Exception($"Neither ${nameof(DOC_SOURCE_DIR)} nor ${nameof(DOC_GIT_FETCH_URL)} were set");
            }

            config.AddSearch(Path.Combine(TEMP_DIR, "search"));
            config.UseCallback(_ => new DocEventListener());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            ApplicationServices = app.ApplicationServices;

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                AllowedHosts = { "*" },
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseMiddleware<InitializeMiddleware>();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseResponseCaching();
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseMvc();

            // Trigger documentation initialization
            app.ApplicationServices.GetRequiredService<IApplicationLifetime>()
                .ApplicationStarted
                .Register(() => Task.Run(async () =>
                {
                    var port = new Uri(LISTEN_URL).Port;

                    using (var httpClient = new HttpClient())
                    {
                        var response = await httpClient.GetAsync($"http://localhost:{port}");
                        _startupTimer.Stop();

                        Log.Information("Application startup time: {T:F1}sec", _startupTimer.Elapsed.TotalSeconds);
                        if (!response.IsSuccessStatusCode)
                        {
                            Log.Warning("Root URL responded with {Status} {Reason}", (int)response.StatusCode, response.ReasonPhrase);
                        }
                    }

                    var delay = 2500 + new Random().Next(1000, 2500);
                    await Task.Delay(delay);
                    app.ApplicationServices.GetRequiredService<IMarkDocService>().Synchronize();
                }));
        }
    }
}
