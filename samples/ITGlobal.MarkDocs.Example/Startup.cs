using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
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
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
            Env = env;
        }

        public IHostingEnvironment Env { get; }
        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add MarkDocs services
            services.AddMarkDocs(config =>
            {
                config.SourceTree.UseStaticDirectory(
                    Path.GetFullPath(Path.Combine(Env.ContentRootPath, "../../docs")),
                    watchForChanges: true
                );

                config.Format.UseMarkdown(markdown =>
                {
                    markdown.UseCodecogsMathRenderer();
                    markdown.CodeBlocks.UseServerSideHighlightJs(Path.Combine(Env.ContentRootPath, "Data", "temp"));
                    markdown.CodeBlocks.UsePlantUmlWebService();
                    markdown.UseResourceUrlResolver<ResourceUrlResolver>();
                });
                
                config.Cache.UseDisk(Path.Combine(Env.ContentRootPath, "Data", "cached-content"));

                config.Extensions.AddTags();

                config.Extensions.AddSearch(Path.Combine(Env.ContentRootPath, "Data", "search-index"));

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
            public string ResolveUrl(IPageRenderContext context, IResourceId resource) 
                => $"/{context.AssetTree.Id}{resource.Id}";
        }
    }
}


