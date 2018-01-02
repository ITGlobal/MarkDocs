using System.IO;
using System.Threading.Tasks;
using ITGlobal.MarkDocs.Format;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Blog.Example
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Env = env;
        }

        public IHostingEnvironment Env { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddMarkdocsBlogEngine(
                Path.Combine(Env.ContentRootPath, "Data"),
                new ResourceUrlResolver(),
                config =>
                {
                    config.UseSourceDirectory(Path.Combine(Env.ContentRootPath, "Blog"));
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IBlogEngine engine)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            Task.Factory.StartNew(engine.Initialize);
        }

        private sealed class ResourceUrlResolver : IResourceUrlResolver
        {
            public string ResolveUrl(IResource resource, IResource relativeTo) => $"{resource.Id}";
        }
    }
}
