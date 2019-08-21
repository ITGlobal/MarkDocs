using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using ITGlobal.MarkDocs.Tools.Serve.Middlewares;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace ITGlobal.MarkDocs.Tools.Serve
{
    public class Startup :StartupBase
    {
        public static IServiceProvider ApplicationServices { get; private set; }
        public static IServerConfig Config { get; set; }

        private readonly Stopwatch _startupTimer = Stopwatch.StartNew();

        [UsedImplicitly]
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddMarkDocs(Config.Configure);
            services.AddResponseCaching();
            services.AddResponseCompression();

            services.Configure<RazorViewEngineOptions>(o =>
            {
                o.ViewLocationFormats.Clear(); 
                o.ViewLocationFormats.Add("/Serve/Views/{1}/{0}" + RazorViewEngine.ViewExtension);
                o.ViewLocationFormats.Add("/Serve/Views/Shared/{0}" + RazorViewEngine.ViewExtension);
#if DEBUG
                o.AllowRecompilingViewsOnFileChange = true;
#endif
            });
        }

        [UsedImplicitly]
        public override void Configure(IApplicationBuilder app)
        {
            ApplicationServices = app.ApplicationServices;

            var env = app.ApplicationServices.GetRequiredService<IHostingEnvironment>();
            var lifetime = app.ApplicationServices.GetRequiredService<IApplicationLifetime>();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                AllowedHosts = { "*" },
                ForwardedHeaders = ForwardedHeaders.All
            });
            app.UseMiddleware<InitializeMiddleware>();

#if DEBUG
            app.UseDeveloperExceptionPage();
#endif

            app.UseResponseCaching();
            app.UseResponseCompression();
            app.UseStaticFiles();
            app.UseMvc();

            // Trigger documentation initialization
            lifetime.ApplicationStarted
                .Register(() => Task.Run(async () =>
                {
                    var port = new Uri(Config.ListenUrl).Port;

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
                }));
        }
    }
}
