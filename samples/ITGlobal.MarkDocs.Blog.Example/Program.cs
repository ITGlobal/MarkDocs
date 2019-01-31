using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace ITGlobal.MarkDocs.Blog.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext} -> {Message}{NewLine}{Exception}")
                .Filter.ByExcluding(ExclusionPredicate)
                .MinimumLevel.Verbose()
                .CreateLogger();

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging(_=>_.AddSerilog())
                .UseStartup<Startup>()
                .Build();

            host.Run();

            bool ExclusionPredicate(LogEvent e)
            {
                if (e.Level >= LogEventLevel.Warning)
                {
                    return false;
                }

                var sourceContext = (e.Properties["SourceContext"] as ScalarValue)?.Value as string;
                if (string.IsNullOrEmpty(sourceContext))
                {
                    return false;
                }

                return sourceContext.StartsWith("Microsoft") || sourceContext.StartsWith("System");
            }
        }
    }
}
