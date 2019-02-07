using Microsoft.AspNetCore.Hosting;
using Serilog;
using System.IO;
using Serilog.Events;

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
                .Filter.ByExcluding(_=>_.Properties.TryGetValue("SourceContext", out var c) && c is ScalarValue s && s.Value is string str && str.StartsWith("Microsoft"))
                .CreateLogger();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureLogging(logBuilder => logBuilder.AddSerilog())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
