using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

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
                config =>
                {
                    config.UseRootUrl("/");
                    config.FromSourceDirectory(Path.Combine(Env.ContentRootPath, "Blog"));
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
                    app.ApplicationServices.GetRequiredService<IBlogEngine>();
                });
        }
    }
}
