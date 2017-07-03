using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ITGlobal.MarkDocs.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext} -> {Message}{NewLine}{Exception}")
                .MinimumLevel.Verbose()
                .CreateLogger();

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseLoggerFactory(loggerFactory)
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
