using System;
using System.IO;
using System.Reflection;
using ITGlobal.CommandLine;
using ITGlobal.MarkDocs.Tools.Generate;
using ITGlobal.MarkDocs.Tools.Lint;
using Serilog;

namespace ITGlobal.MarkDocs.Tools
{
    public static class Program
    {
        public static string Version => typeof(Program)
            .GetTypeInfo()
            .Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion;

        public static int Main(string[] args)
        {
            return CLI.HandleErrors(() =>
            {
                var parser = CLI.Parser();
                parser.ExecutableName("markdocs");
                parser.Title("Markdocs command line tool");
                parser.Version(Version);
                parser.SuppressLogo(true);

                var verboseSwitch = parser.Switch("v").Alias("verbose").HelpText("Enable verbose output");
                var helpSwitch = parser.Switch("help").HelpText("Display help");
                var versionSwitch = parser.Switch("version").HelpText("Display version number");

                parser.Callback(_ =>
                {
                    if (versionSwitch.IsSet)
                    {
                        Console.WriteLine(Version);
                        return 0;
                    }

                    if (helpSwitch.IsSet)
                    {
                        parser.Usage().Print();
                        return 0;
                    }

                    parser.Usage().Print();

                    return 0;
                });

                {
                    var lintCommand = parser.Command("lint")
                        .HelpText("Run a linter on a documentation");

                    var pathParameter = lintCommand
                        .Parameter<string>(0, "path")
                        .DefaultValue(".")
                        .HelpText("Path to documentation root directory");
                    var summarySwitch = lintCommand.Switch("summary").HelpText("Display summary");

                    lintCommand.Callback(_ =>
                    {
                        if (versionSwitch.IsSet)
                        {
                            Console.WriteLine(Version);
                            return 0;
                        }

                        if (helpSwitch.IsSet)
                        {
                            parser.Usage("lint").Print();
                            return 0;
                        }

                        SetupLogger(verboseSwitch.IsSet);
                        var path = Path.GetFullPath(pathParameter.Value);
                        Linter.Run(path, verboseSwitch.IsSet, summarySwitch.IsSet);

                        return 0;
                    });
                }

                {
                    var buildCommand = parser.Command("build")
                        .HelpText("Generate static website from documentation");

                    var pathParameter = buildCommand
                        .Parameter<string>(0, "path")
                        .DefaultValue(".")
                        .HelpText("Path to documentation root directory");
                    var targetDirParameter = buildCommand
                        .Parameter<string>("o")
                        .Alias("output")
                        .HelpText("Path to output directory");
                    var templateParameter = buildCommand
                        .Parameter<string>("t")
                        .Alias("template")
                        .HelpText("Template name");

                    buildCommand.Callback(_ =>
                    {
                        if (versionSwitch.IsSet)
                        {
                            Console.WriteLine(Version);
                            return 0;
                        }

                        if (helpSwitch.IsSet)
                        {
                            parser.Usage("build").Print();
                            return 0;
                        }

                        SetupLogger(verboseSwitch.IsSet);

                        var path = Path.GetFullPath(pathParameter.Value);
                        var outputPath = targetDirParameter.IsSet
                            ? targetDirParameter.Value
                            : Path.Combine(path, "__output__");
                        var templateName = templateParameter.IsSet
                            ? templateParameter.Value
                            : "";

                        return Generator.Run(
                            path, 
                            outputPath,
                            templateName,
                            verboseSwitch.IsSet
                        );
                    });
                }

                return parser.Parse(args).Run();
            });
        }

        public static void PrintReport(ICompilationReport report, bool verbose)
        {
            if (report.Common.Count == 0 && report.Pages.Count == 0)
            {
                using (CLI.WithForeground(ConsoleColor.Green))
                {
                    Console.WriteLine("Everything is OK");
                }
                return;
            }

            foreach (var error in report.Common)
            {
                switch (error.Type)
                {
                    case CompilationReportMessageType.Warning:
                        using (CLI.WithForeground(ConsoleColor.Yellow))
                        {
                            Console.Write("warning: ");
                        }
                        break;
                    case CompilationReportMessageType.Error:
                        using (CLI.WithForeground(ConsoleColor.Red))
                        {
                            Console.Write("error: ");
                        }
                        break;
                    default:
                        using (CLI.WithForeground(ConsoleColor.Magenta))
                        {
                            Console.Write($"{error.Type:G}: ");
                        }
                        break;
                }

                Console.WriteLine(error.Message);
                if (error.Exception != null && verbose)
                {
                    PrintException(error.Exception);
                }
            }

            foreach (var page in report.Pages)
            {
                foreach (var error in page.Messages)
                {
                    Console.Write(page.SourceFileName);
                    if (error.LineNumber != null)
                    {
                        Console.Write(":");
                        Console.Write(error.LineNumber.Value + 1);
                    }

                    Console.Write(": ");

                    switch (error.Type)
                    {
                        case CompilationReportMessageType.Warning:
                            using (CLI.WithForeground(ConsoleColor.Yellow))
                            {
                                Console.Write("warning: ");
                            }
                            break;
                        case CompilationReportMessageType.Error:
                            using (CLI.WithForeground(ConsoleColor.Red))
                            {
                                Console.Write("error: ");
                            }
                            break;
                        default:
                            using (CLI.WithForeground(ConsoleColor.Magenta))
                            {
                                Console.Write($"{error.Type:G}: ");
                            }
                            break;
                    }

                    Console.WriteLine(error.Message);
                    if (error.Exception != null && verbose)
                    {
                        PrintException(error.Exception);
                    }
                }
            }
        }

        private static void PrintException(Exception exception)
        {
            if (exception != null)
            {
                var e = exception;
                while (e != null)
                {
                    if (e != exception)
                    {
                        Console.WriteLine("----------");
                    }

                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);

                    e = e.InnerException;
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
                configuration.MinimumLevel.Error();
            }

            Log.Logger = configuration.CreateLogger();
        }
    }
}