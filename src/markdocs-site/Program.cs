using System;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace ITGlobal.MarkDocs.Site
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureSerilog();
            Env.Initialize();
            Run(CreateWebHostBuilder(args).Build());
            
            Log.Information("Good bye");
            Log.CloseAndFlush();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .ConfigureLogging(ConfigureAspnetLogger)
                .UseUrls(Env.LISTEN_URL)
                .UseStartup<Startup>();
        }

        private static void ConfigureAspnetLogger(ILoggingBuilder logBuilder)
        {
            logBuilder.ClearProviders();

            logBuilder.AddFilter("System", level => level >= LogLevel.Warning);
            logBuilder.AddFilter("Microsoft", level => level >= LogLevel.Warning);

            logBuilder.AddSerilog();
        }

        private static void ConfigureSerilog()
        {
            var config = new LoggerConfiguration();
            config.MinimumLevel.Is(Env.LOG_LEVEL);

            const string outputTemplate = "[{Level:u3} {ThreadId}] {Message:lj}{NewLine}{Exception}";
            //const string outputTemplate = "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}";
            config.WriteTo.Console(outputTemplate: outputTemplate, standardErrorFromLevel: LogEventLevel.Warning);

            config.Enrich.FromLogContext();
            config.Enrich.WithThreadId();

            config.Filter.ByExcluding(ExcludePredicate);

            Log.Logger = config.CreateLogger();

            bool ExcludePredicate(LogEvent e)
            {
                if (e.Properties.TryGetValue("SourceContext", out var value) &&
                    value is ScalarValue scalarValue &&
                    scalarValue.Value is string sourceContext)
                {
                    if (sourceContext.StartsWith("Microsoft.") ||
                        sourceContext.StartsWith("System."))
                    {
                        return e.Level <= LogEventLevel.Information;
                    }
                }

                return false;
            }
        }

        private static void Run(IWebHost host)
        {
            using (host)
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
        }
    }
}
