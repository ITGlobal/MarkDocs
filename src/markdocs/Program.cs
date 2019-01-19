using System;
using System.IO;
using System.Reflection;
using ITGlobal.CommandLine;
using ITGlobal.CommandLine.Parsing;
using ITGlobal.MarkDocs.Tools.Generate;
using ITGlobal.MarkDocs.Tools.Lint;
using Serilog;
using static ITGlobal.CommandLine.Terminal;

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
            return TerminalErrorHandler.Handle(() =>
            {
                var parser = CliParser.NewTreeParser();
                parser.ExecutableName("markdocs");
                parser.HelpText("Markdocs command line tool");
                parser.SuppressLogo(true);

                var verboseSwitch = parser.Switch('v', "verbose").HelpText("Enable verbose output");
                var versionSwitch = parser.Switch("version").HelpText("Display version number");

                parser.BeforeExecute(ctx =>
                {
                    if (versionSwitch.IsSet)
                    {
                        Stdout.WriteLine(Version);
                        ctx.Break();
                    }

                    SetupLogger(verboseSwitch.IsSet);
                });

                {
                    var lintCommand = parser.Command("lint")
                        .HelpText("Run a linter on a documentation");

                    var pathParameter = lintCommand
                        .Argument("path", 0)
                        .DefaultValue(".")
                        .HelpText("Path to documentation root directory")
                        .Required();

                    var summarySwitch = lintCommand
                        .Switch("summary")
                        .HelpText("Display summary");

                    lintCommand.OnExecute(_ =>
                    {
                        SetupLogger(verboseSwitch.IsSet);
                        var path = Path.GetFullPath(pathParameter.Value);
                        Linter.Run(path, verboseSwitch.IsSet, summarySwitch.IsSet);
                    });
                }

                {
                    var buildCommand = parser.Command("build")
                        .HelpText("Generate static website from documentation");

                    var pathParameter = buildCommand
                        .Argument("path",0)
                        .DefaultValue(".")
                        .HelpText("Path to documentation root directory")
                        .Required();
                    var targetDirParameter = buildCommand
                        .Option('o', "output")
                        .HelpText("Path to output directory");
                    var templateParameter = buildCommand
                        .Option('t', "template")
                        .HelpText("Template name");

                    buildCommand.OnExecute(ctx =>
                    {
                        

                        var path = Path.GetFullPath(pathParameter.Value);
                        var outputPath = targetDirParameter.IsSet
                            ? targetDirParameter.Value
                            : Path.Combine(path, "__output__");
                        var templateName = templateParameter.IsSet
                            ? templateParameter.Value
                            : "";

                        var exitCode = Generator.Run(
                            path,
                            outputPath,
                            templateName,
                            verboseSwitch.IsSet
                        );
                        ctx.Break(exitCode);
                    });
                }

                return parser.Parse(args).Run();
            });
        }

        public static void PrintReport(ICompilationReport report, bool verbose)
        {
            if (report.Common.Count == 0 && report.Pages.Count == 0)
            {
                Stdout.WriteLine("Everything is OK".WithForeground(ConsoleColor.Green));
                return;
            }

            foreach (var error in report.Common)
            {
                switch (error.Type)
                {
                    case CompilationReportMessageType.Warning:
                        Stdout.Write("warning: ".WithForeground(ConsoleColor.Yellow));
                        break;
                    case CompilationReportMessageType.Error:
                        Stdout.Write("error: ".WithForeground(ConsoleColor.Red));
                        break;
                    default:
                        Stdout.Write($"{error.Type:G}: ".WithForeground(ConsoleColor.Magenta));
                        break;
                }

                Stdout.WriteLine(error.Message);
                if (error.Exception != null && verbose)
                {
                    PrintException(error.Exception);
                }
            }

            foreach (var page in report.Pages)
            {
                foreach (var error in page.Messages)
                {
                    Stdout.Write(page.SourceFileName);
                    if (error.LineNumber != null)
                    {
                        Stdout.Write(":");
                        Stdout.Write(error.LineNumber.Value + 1);
                    }

                    Stdout.Write(": ");

                    switch (error.Type)
                    {
                        case CompilationReportMessageType.Warning:
                            Stdout.Write("warning: ".WithForeground(ConsoleColor.Yellow));
                            break;
                        case CompilationReportMessageType.Error:
                            Stdout.Write("error: ".WithForeground(ConsoleColor.Red));
                            break;
                        default:
                            Stdout.Write($"{error.Type:G}: ".WithForeground(ConsoleColor.Magenta));
                            break;
                    }

                    Stdout.WriteLine(error.Message);
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
                        Stdout.WriteLine("----------");
                    }

                    Stdout.WriteLine(e.Message);
                    Stdout.WriteLine(e.StackTrace);

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