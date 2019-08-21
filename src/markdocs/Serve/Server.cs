using System;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ITGlobal.MarkDocs.Tools.Serve
{
    public static class Server
    {
        public static int Run(IServerConfig config)
        {
            Startup.Config = config;
            var builder = WebHost.CreateDefaultBuilder()
                .ConfigureLogging(log =>
                {
                    log.ClearProviders();
                    log.AddFilter("System", level => level >= LogLevel.Warning);
                    log.AddFilter("Microsoft", level => level >= LogLevel.Warning);
                    log.AddSerilog();
                })
                .UseUrls(config.ListenUrl)
                .UseContentRoot(Path.GetDirectoryName(typeof(Server).Assembly.Location))
                .UseStartup<Startup>();
            using (var host = builder.Build())
            {
                host.Start();

                var service = host.Services.GetService<IHostingEnvironment>();

                Log.Debug("Hosting environment: {0}", service.EnvironmentName);
                Log.Debug("Content root path: {0}", service.ContentRootPath);

                var addressesFeature = host.ServerFeatures.Get<IServerAddressesFeature>();
                var addresses = addressesFeature?.Addresses;
                if (addresses != null)
                {
                    if (addresses.Count == 1)
                    {
                        Log.Information("Now listening on: {0}", addresses.First());
                    }
                    else
                    {
                        Log.Information("Now listening on: [ {0} ]", addresses);
                    }
                }

                var exit = new ManualResetEventSlim();

                Console.CancelKeyPress += (_, e) =>
                {
                    exit.Set();
                    e.Cancel = true;
                };

                Log.Information("Press {Key} to exit", "<Ctrl+C>");
                exit.Wait();
                Log.Information("Shutting down");
            }

            return Environment.ExitCode;
        }
    }
}
