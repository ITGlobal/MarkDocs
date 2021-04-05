using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using ITGlobal.CommandLine;
using ITGlobal.CommandLine.Parsing;
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

            var cacheDir = parser.Option("cache").HelpText("Path to cache directory");
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

            LintCommand.Setup(parser, cacheDir, quiet);
            BuildCommand.Setup(parser, cacheDir, quiet);
            ServeCommand.Setup(parser, cacheDir, verbosity, quiet);

            return TerminalErrorHandler.Handle(() => parser.Parse(args).Run());
        }

        public static string DetectTempDir(string tempPath, string source = null, string suffix = null)
        {
            string path;
            if (!string.IsNullOrWhiteSpace(tempPath))
            {
                path = Path.GetFullPath(tempPath);
                if (path != tempPath)
                {
                    Log.Debug("Cache path \"{Path}\" resolved to \"{FullPath}\"", tempPath, path);
                }
            }
            else
            {
                path = Path.Combine(Path.GetTempPath(), "markdocs");

                if (!string.IsNullOrWhiteSpace(suffix))
                {
                    path = Path.Combine(path, suffix);
                }

                if (!string.IsNullOrWhiteSpace(source))
                {
                    string hash;
                    using (var md5 = MD5.Create())
                    {
                        hash = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(source)))
                            .Replace("-", "")
                            .ToLowerInvariant();
                    }
                    path = Path.Combine(path, hash);
                }

                Log.Warning("Auto-detected cache path \"{Path}\"", path);
            }

            Directory.CreateDirectory(path);
            return path;
        }

        public static void PrintReport(ICompilationReport report, bool summary = false, bool quiet = false)
        {
            if (quiet)
            {
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
                return;
            }

            Console.Error.WriteLine();
            Console.Error.WriteLine("Compilation report".Cyan());
            Console.Error.WriteLine("==================".DarkCyan());
            Console.Error.WriteLine();

            if (report.Messages.Count == 0)
            {
                Console.Error.WriteLine("Everything is OK".Green());
            }
            else
            {
                foreach (var (filename, list) in report.Messages)
                {
                    Console.Error.WriteLine($"* ./{filename}".White());
                    foreach (var error in list)
                    {
                        Console.Error.Write("  ");
                        if (error.LineNumber != null)
                        {
                            Console.Error.Write($"(at {error.LineNumber.Value + 1}) ".DarkGray());
                        }

                        switch (error.Type)
                        {
                            case CompilationReportMessageType.Warning:
                                Console.Error.Write("WARNING: ".Yellow());
                                break;
                            case CompilationReportMessageType.Error:
                                Console.Error.Write("ERROR: ".Red());
                                break;
                            default:
                                Console.Error.Write($"{error.Type:G}: ".Magenta());
                                break;
                        }

                        Console.Error.WriteLine(error.Message);
                    }
                }
            }

            Console.Error.WriteLine();

            if (summary)
            {
                var errors = report.Messages.Values.Sum(_ => _.Count(x => x.Type == CompilationReportMessageType.Error)); ;
                var warnings = report.Messages.Values.Sum(_ => _.Count(x => x.Type == CompilationReportMessageType.Warning));

                Console.Error.WriteLine("Summary".Yellow());
                Console.Error.WriteLine("-------".DarkYellow());
                Console.Error.WriteLine($"  {errors} error(s)");
                Console.Error.WriteLine($"  {warnings} warning(s)");
                Console.Error.WriteLine();
            }
        }

        private static void SetupLogger(int verbosity, bool quiet)
        {
            const string outputTemplate = "[{Level:u3} {ThreadId}] {Message:lj}{NewLine}{Exception}";

            var configuration = new LoggerConfiguration();
            configuration.Enrich.FromLogContext();
            configuration.Enrich.WithThreadId();
            configuration.Filter.ByExcluding(ExcludePredicate);
            configuration.WriteTo.Console(
                outputTemplate: outputTemplate,
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
                        configuration.MinimumLevel.Information();
                        break;
                    case 2:
                        configuration.MinimumLevel.Debug();
                        break;
                    default:
                        configuration.MinimumLevel.Verbose();
                        break;
                }
            }

            Log.Logger = configuration.CreateLogger();

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
    }
}