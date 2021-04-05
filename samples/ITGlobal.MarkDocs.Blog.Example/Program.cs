using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;
using System.IO;

namespace ITGlobal.MarkDocs.Blog.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext} -> {Message}{NewLine}{Exception}")
                .MinimumLevel.Verbose()
                .Filter.ByExcluding(_ => _.Properties.TryGetValue("SourceContext", out var c) && c is ScalarValue s && s.Value is string str && str.StartsWith("Microsoft"))
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
