using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;

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

            //.ConfigureLogging(_ => _.AddSerilog().AddFilter((category, logLevel) => (category.StartsWith("Microsoft") && logLevel >= LogLevel.Trace)))
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging(logBuilder =>
                {
                    logBuilder.AddFilter((provider, category, logLevel) =>
                    {
                        if (category.StartsWith("Microsoft") && logLevel<= LogLevel.Error)
                        {
                            return false;
                        }
                        return true;
                    });
                    //_.AddFilter("Microsoft.*", LogLevel.Error);
                    // logBuilder.AddSerilog();
                    logBuilder.AddConsole();
                })
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
