using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Comments;
using ITGlobal.MarkDocs.Git;
using ITGlobal.MarkDocs.Storage;
using ITGlobal.MarkDocs.Tags;
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
                config.Format.UseMarkdown(new MarkdownOptions
                {
                    ResourceUrlResolver = new ResourceUrlResolver(),
                    SyntaxColorizer = new ServerHighlightJsSyntaxColorizer(Path.Combine(Env.ContentRootPath, "Data", "temp"))
                });
                config.Cache.UseDisk(Path.Combine(Env.ContentRootPath, "Data", "cached-content"), enableConcurrentWrites: false);

                config.Storage.UseStaticDirectory(Path.GetFullPath(Path.Combine(Env.ContentRootPath, "../../docs")), enableWatch: true);
                
                config.Extensions.AddLiteDbComments(Path.Combine(Env.ContentRootPath, "Data", "comments.dat"));
                config.Extensions.AddTags();
            });
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            IApplicationLifetime appLifetime,
            IMarkDocService markDocs)
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

            Task.Factory.StartNew(markDocs.Initialize);
        }

        private sealed class ResourceUrlResolver : IResourceUrlResolver
        {
            public string ResolveUrl(IResource resource, IResource relativeTo)
                 => $"/{resource.Documentation.Id}{resource.Id}";
        }
    }
}


