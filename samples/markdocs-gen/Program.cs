using System;
using System.Reflection;
using ITGlobal.CommandLine;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Storage;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ITGlobal.MarkDocs.StaticGen
{
    public class Program
    {
        public static string Version => typeof(Program)
            .GetTypeInfo()
            .Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;

        public static int Main(string[] args)
        {
            try
            {
                var parser = CLI.Parser();
                parser.ExecutableName("markdocs-gen");
                parser.Title("Markdocs static site generator");
                parser.Version(Version);

                var verboseSwitch = parser.Switch("v").Alias("verbose").HelpText("Enable verbose output");
                var srcPath = parser.Parameter<string>(0, "src").Required().HelpText("Path to source directory");
                var destPath = parser.Parameter<string>(1, "dest").Required().HelpText("Path to destination directory"); ;
                var template = parser.Parameter<string>("t").Alias("template").DefaultValue("minimal").HelpText("Template name");
                var watchSwitch = parser.Switch("watch").HelpText("Watch for changes and rebuild them on fly");
                var serveSwitch = parser.Switch("serve").HelpText("Enable built-in HTTP server for live preview");
                var helpSwitch = parser.Switch("help").HelpText("Display help");

                parser.Callback(_ =>
                {
                    if (helpSwitch.IsSet)
                    {
                        parser.Usage().Print();
                        return 0;
                    }

                    SetupLogger(verboseSwitch.IsSet);
                    Generate(srcPath.Value, destPath.Value, ResolveTemplate(template), watchSwitch.IsSet, serveSwitch.IsSet);

                    return 0;
                });

               
                return CLI.HandleErrors(() =>
                {
                    try
                    {
                        return parser.Parse(args).Run();
                    }
                    catch (CommandLineValidationException e)
                    {
                        parser.Usage().Print();
                        throw;
                    }
                });
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void Generate(string sourceDir, string targetDir, ITemplate template, bool enableWatch, bool enableHttpServer)
        {
            Console.WriteLine();
            Console.Write("Generating static website from ");
            using (CLI.WithForeground(ConsoleColor.Cyan))
            {
                Console.Write(sourceDir);
            }
            Console.Write(" into ");
            using (CLI.WithForeground(ConsoleColor.Cyan))
            {
                Console.Write(targetDir);
            }
            Console.Write(" using ");
            using (CLI.WithForeground(ConsoleColor.Cyan))
            {
                Console.Write(template.Name);
            }
            Console.WriteLine(" template");


            using (var service = Configure(sourceDir, targetDir, template, enableWatch))
            {
                service.Initialize();

                if (!enableWatch)
                {
                    return;
                }

                IWebHost host = null;
                if (enableHttpServer)
                {
                    host = CreateHttpServer(targetDir);
                }

                using (host)
                {
                    Console.WriteLine("Watching for changes...");
                    host?.Run();

                    while (true)
                    {
                        Console.ReadKey(true);
                    }
                }
            }
        }

        private static void SetupLogger(bool verbose)
        {
            var configuration = new LoggerConfiguration();
            configuration.Enrich.FromLogContext();
            configuration.WriteTo.LiterateConsole(outputTemplate: "{Message}{NewLine}{Exception}");
            if (verbose)
            {
                configuration.MinimumLevel.Verbose();
            }
            else
            {
                configuration.MinimumLevel.Fatal();
            }

            Log.Logger = configuration.CreateLogger();
        }

        private static IWebHost CreateHttpServer(string targetDir)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseLoggerFactory(new LoggerFactory())
                .Configure(app =>
                {
                    app.UseFileServer(new FileServerOptions
                    {
                        FileProvider = new PhysicalFileProvider(targetDir)
                    });
                })
                .Build();

            Console.Write("Open ");
            using (CLI.WithForeground(ConsoleColor.Cyan))
            {
                Console.Write("http://127.0.0.1:5000");
            }
            Console.WriteLine(" for live preview");

            return host;
        }

        private static IMarkDocService Configure(
            string sourceDir,
            string targetDir,
            ITemplate template,
            bool enableWatch = false)
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddSerilog(Log.Logger);

            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory>(loggerFactory);
            services.AddMarkDocs(config =>
            {
                config.UseCallback(new EventCallback());
                config.Storage.UseStaticDirectory(sourceDir, enableWatch: enableWatch);
                config.Cache.Use(sp => sp.AddSingleton<ICache>(new OutputCache(targetDir, template)));
                config.Format.UseMarkdown(resourceUrlResolverFactory: _ => new ResourceUrlResolver());
            });

            var provider = new ServiceProvider(services);
            var service = provider.GetRequiredService<IMarkDocService>();
            return service;
        }

        private static ITemplate ResolveTemplate(INamedParameter<string> parameter)
        {
            switch (parameter.Value)
            {
                case "preview":
                    return new PreviewTemplate();
                default:
                    return new MinimalBootstrapTemplate();

            }
        }
    }
}
