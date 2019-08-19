using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Tags;
using ITGlobal.MarkDocs.Search;
using ITGlobal.MarkDocs.Source;
using Serilog;

namespace ITGlobal.MarkDocs.Example
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Env = env;
        }

        public IHostingEnvironment Env { get; }

        // This method gets called by the runtime. AddExtension this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add MarkDocs services
            services.AddMarkDocs(config =>
            {
                config.FromStaticDirectory(
                    //Path.GetFullPath(Path.Combine(Env.ContentRootPath, "../../docs")),
                    @"d:\crex\techdocs",
                    watchForChanges: true
                );
                config.UseResourceUrlResolver<ResourceUrlResolver>();

                config.UseMarkdown(markdown =>
                {
                    markdown.AddCodecogsMathRenderer();
                    markdown.AddHighlightJs(Path.Combine(Env.ContentRootPath, "Data", "hljs"));
                    markdown.AddPlantUmlWebService();
                });
                
                config.UseDiskCache(Path.Combine(Env.ContentRootPath, "Data", "cache"));

                config.AddTags();
                config.AddSearch(Path.Combine(Env.ContentRootPath, "Data", "search"));

                config.UseAspNetLog();
            });
        }

        public void Configure(IApplicationBuilder app, IApplicationLifetime appLifetime)
        {
            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
            app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            appLifetime.ApplicationStarted.Register(
                    () =>
                    {
                        app.ApplicationServices.GetRequiredService<IMarkDocService>();
                    });
        }

        private sealed class ResourceUrlResolver : IResourceUrlResolver
        {
            public string ResolveUrl(IResourceUrlResolutionContext context, IResourceId resource)
                => $"/{context.SourceTreeId}{resource.Id}";
        }
    }
}



