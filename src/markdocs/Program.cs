using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ITGlobal.CommandLine;
using ITGlobal.CommandLine.Parsing;
using ITGlobal.MarkDocs.Tools.Generate;
using ITGlobal.MarkDocs.Tools.Lint;
using Serilog;
using Serilog.Events;

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
            var parser = CliParser.NewTreeParser();
                parser.ExecutableName("markdocs");
                parser.HelpText("Markdocs command line tool");
                parser.SuppressLogo();

                var tempDir = parser.Option("temp-dir").HelpText("Path to temp directory");
                var verbosity = parser.RepeatableSwitch('v', "verbose").HelpText("Enable verbose output");
                var quiet = parser.Switch('q', "quiet").HelpText("Enable quiet output");
                var version = parser.Switch("version").HelpText("Display version number");

                parser.BeforeExecute(ctx =>
                {
                    if (version)
                    {
                        Console.Out.WriteLine(Version);
                        ctx.Break();
                    }

                    SetupLogger(verbosity, quiet);
                });

                {
                    var lintCommand = parser.Command("lint")
                        .HelpText("Run a linter on a documentation");

                    var pathParameter = lintCommand
                        .Argument("path")
                        .DefaultValue(".")
                        .HelpText("Path to documentation root directory")
                        .Required();

                    var summary = lintCommand
                        .Switch('s', "summary")
                        .HelpText("Display summary");

                    lintCommand.OnExecute(_ =>
                    {
                        var path = Path.GetFullPath(pathParameter);
                        Linter.Run(path, DetectTempDir(tempDir, path), quiet, summary);
                    });
                }

                {
                    var buildCommand = parser.Command("build")
                        .HelpText("Generate static website from documentation");

                    var pathParameter = buildCommand
                        .Argument("path")
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
                            sourceDir: path,
                            targetDir: outputPath,
                            templateName: templateName,
                            tempDir: DetectTempDir(tempDir, path),
                            quiet: quiet
                        );
                        ctx.Break(exitCode);
                    });
                }

            return TerminalErrorHandler.Handle(() =>
            {
                
                return parser.Parse(args).Run();
            });
        }

        private static string DetectTempDir(string tempPath, string sourcePath)
        {
            if (!string.IsNullOrWhiteSpace(tempPath))
            {
                return tempPath;
            }

            string hash;
            using (var md5 = MD5.Create())
            {
                hash=BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(sourcePath)))
                    .Replace("-", "")
                    .ToLowerInvariant();
            }

            return Path.Combine(Path.GetTempPath(), "markdocs", "wrk", hash);
        }

        public static void PrintReport(ICompilationReport report, bool quiet)
        {
            if (report.Messages.Count == 0)
            {
                Console.Error.WriteLine("Everything is OK".Green());
                return;
            }

            foreach (var (filename, list) in report.Messages)
            {
                foreach (var error in list)
                {
                    Console.Error.Write(filename);
                    if (error.LineNumber != null)
                    {
                        Console.Error.Write($":{error.LineNumber.Value + 1}");
                    }

                    Console.Error.Write(": ");

                    switch (error.Type)
                    {
                        case CompilationReportMessageType.Warning:
                            Console.Error.Write("warning: ".Yellow());
                            break;
                        case CompilationReportMessageType.Error:
                            Console.Error.Write("error: ".Red());
                            break;
                        default:
                            Console.Error.Write($"{error.Type:G}: ".Magenta());
                            break;
                    }

                    Console.Error.WriteLine(error.Message);
                }
            }
        }

        private static void SetupLogger(int verbosity, bool quiet)
        {
            var configuration = new LoggerConfiguration();
            configuration.Enrich.FromLogContext();
            configuration.WriteTo.LiterateConsole(
                outputTemplate: "{Message}{NewLine}{Exception}",
                standardErrorFromLevel: LogEventLevel.Verbose
            );

            if (quiet)
            {
                configuration.MinimumLevel.Fatal();
            }
            else
            {
                switch (verbosity)
                {
                    case 0:
                        configuration.MinimumLevel.Error();
                        break;
                    case 1:
                        configuration.MinimumLevel.Warning();
                        break;
                    case 2:
                        configuration.MinimumLevel.Information();
                        break;
                    case 3:
                        configuration.MinimumLevel.Debug();
                        break;
                    default:
                        configuration.MinimumLevel.Verbose();
                        break;
                }
            }

            Log.Logger = configuration.CreateLogger();
        }
    }
}