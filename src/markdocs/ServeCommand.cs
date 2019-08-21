using System;
using System.IO;
using System.Threading;
using ITGlobal.CommandLine;
using ITGlobal.CommandLine.Parsing;
using ITGlobal.MarkDocs.Cache;
using ITGlobal.MarkDocs.Format;
using ITGlobal.MarkDocs.Search;
using ITGlobal.MarkDocs.Source;
using ITGlobal.MarkDocs.Tools.Serve;
using ITGlobal.MarkDocs.Tools.Serve.Extensions;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Serilog;

namespace ITGlobal.MarkDocs.Tools
{
    public static class ServeCommand
    {
        public static void Setup(ICliCommandRoot app, CliOption<string> cacheDirectory, CliRepeatableSwitch verbose, CliSwitch quiet)
        {
            // markdocs serve
            {
                var command = app.Command("serve")
                    .HelpText("Serve website from directory");

                var path = command
                    .Argument("path")
                    .DefaultValue(".")
                    .HelpText("Path to source directory")
                    .Required();
                var listenUrl = command
                    .Option("listen-url")
                    .HelpText("HTTP endpoint to listen")
                    .DefaultValue("http://0.0.0.0:8000")
                    .Required();
                var publicUrl = command
                    .Option("public-url")
                    .HelpText("Public root URL");
                var enableDeveloperMode = command
                    .Switch('d', "dev")
                    .HelpText("Enable developer mode");
                var theme = command
                    .Option<Theme>("theme")
                    .HelpText("Color theme")
                    .DefaultValue(Theme.Light);

                command.OnExecute(ctx =>
                {
                    IServerConfig config;
                    try
                    {
                        config = new DirectoryServerConfig(
                            listenUrl: listenUrl,
                            publicUrl: publicUrl,
                            cacheDirectory: cacheDirectory,
                            enableDeveloperMode: enableDeveloperMode,
                            theme: theme,
                            verbose: verbose,
                            quiet: quiet,
                            sourceDirectory: path
                        );
                    }
                    catch (ValidationException e)
                    {
                        Console.Error.WriteLine(e.Message.Red());
                        ctx.Break(-1);
                        return;
                    }

                    var exitCode = Server.Run(config);
                    ctx.Break(exitCode);
                });
            }

            // markdocs serve-git
            {
                var command = app.Command("serve-git")
                    .HelpText("Serve website from git repository");

                var repo = command
                    .Argument("repo")
                    .HelpText("Path/URL to source git repository")
                    .Required();
                var listenUrl = command
                    .Option("listen-url")
                    .HelpText("HTTP endpoint to listen")
                    .DefaultValue("http://0.0.0.0:8000")
                    .Required();
                var publicUrl = command
                    .Option("public-url")
                    .HelpText("Public root URL");
                var enableDeveloperMode = command
                    .Switch('d', "dev")
                    .HelpText("Enable developer mode");
                var theme = command
                    .Option<Theme>("theme")
                    .HelpText("Color theme")
                    .DefaultValue(Theme.Light);
                var username = command
                    .Option('u', "username")
                    .HelpText("Git username");
                var password = command
                    .Option('p', "password")
                    .HelpText("Git password");
                var branch = command
                    .Option('b', "branch")
                    .HelpText("Git branch")
                    .DefaultValue("master")
                    .Required();
                var pollInterval = command
                    .Option<int>("poll-interval")
                    .HelpText("Git repository polling interval, in seconds")
                    .DefaultValue(5 * 60)
                    .Required();
                var disablePolling = command.Switch("no-poll")
                    .HelpText("Disable git repository polling");

                command.OnExecute(ctx =>
                {
                    IServerConfig config;
                    try
                    {
                        config = new GitServerConfig(
                            listenUrl: listenUrl,
                            publicUrl: publicUrl,
                            cacheDirectory: cacheDirectory,
                            enableDeveloperMode: enableDeveloperMode,
                            theme: theme,
                            verbose: verbose,
                            quiet: quiet,
                            fetchUrl: repo,
                            username: username,
                            password: password,
                            branches: new[] { branch.Value },
                            pollInterval: !disablePolling ? TimeSpan.FromSeconds(pollInterval.Value) : (TimeSpan?)null
                        );
                    }
                    catch (ValidationException e)
                    {
                        Console.Error.WriteLine(e.Message.Red());
                        ctx.Break(-1);
                        return;
                    }

                    var exitCode = Server.Run(config);
                    ctx.Break(exitCode);
                });
            }
        }

        private abstract class ServerConfigBase : IServerConfig
        {
            protected ServerConfigBase(
                string source,
                string listenUrl,
                string publicUrl,
                string cacheDirectory,
                bool enableDeveloperMode,
                Theme theme,
                bool verbose,
                bool quiet)
            {
                listenUrl = ClampTrailingSlashes(listenUrl);
                if (!Uri.TryCreate(listenUrl, UriKind.Absolute, out var u) ||
                    u.Scheme != Uri.UriSchemeHttp ||
                    u.PathAndQuery != "/")
                {
                    throw new ValidationException(
                        $"Invalid \"--listen-url\" value: \"{listenUrl}\" is not a valid endpoint.\n" +
                        $"Expected an endpoint with the following format: \"http://$ip_address:$port\"."
                    );
                }

                publicUrl = ClampTrailingSlashes(publicUrl);
                if (string.IsNullOrWhiteSpace(publicUrl))
                {
                    var url = new UriBuilder(listenUrl);
                    url.Host = "localhost";
                    publicUrl = url.ToString();
                    publicUrl = ClampTrailingSlashes(publicUrl);

                    Log.Warning("\"{Param}\" is not set, falling back to \"{Fallback}\"", "--public-url", publicUrl);
                }

                cacheDirectory = Program.DetectTempDir(cacheDirectory, source: source);

                ListenUrl = listenUrl;
                PublicUrl = publicUrl;
                CacheDirectory = cacheDirectory;
                EnableDeveloperMode = enableDeveloperMode;
                Theme = theme;
                Verbose = verbose;
                Quiet = quiet;
            }

            public string ListenUrl { get; }
            public string PublicUrl { get; }
            public string CacheDirectory { get; }
            public bool EnableDeveloperMode { get; }
            public bool Verbose { get; }
            public bool Quiet { get; }
            public Theme Theme { get; }

            public virtual void Configure(MarkDocsOptions config)
            {
                config.UseDiskCache(Path.Combine(CacheDirectory, "cache"));
                config.UseMarkdown(markdown =>
                {
                    markdown.AddHighlightJs(Path.Combine(CacheDirectory, "hljs"));
                    markdown.AddCodecogsMathRenderer();
                    markdown.AddPlantUmlWebService();
                    markdown.UseTocRenderer<CustomTocRenderer>();
                    markdown.OverrideRendering<LinkInline>(ImageRenderer.RenderImage);
                    markdown.OverrideRendering<QuoteBlock>(QuoteBlockRenderer.Render);
                });
                config.UseResourceUrlResolver(new ResourceUrlResolver());
                config.UseAspNetLog();
                config.AddSearch(Path.Combine(CacheDirectory, "search"));

                config.UseCallback(_ => new DefaultEventListener(Quiet));
                config.AddExtension(_ => new CompilationReportExtensionFactory(Quiet));
            }
        }

        private sealed class DirectoryServerConfig : ServerConfigBase
        {
            public DirectoryServerConfig(
                string listenUrl,
                string publicUrl,
                string cacheDirectory,
                bool enableDeveloperMode,
                Theme theme,
                bool verbose,
                bool quiet,
                string sourceDirectory)
                : base(sourceDirectory, listenUrl, publicUrl, cacheDirectory, enableDeveloperMode, theme, verbose, quiet)
            {
                var fullPath = Path.GetFullPath(sourceDirectory);
                if (sourceDirectory != fullPath)
                {
                    Log.Debug("Source path \"{Path}\" resolved to \"{FullPath}\"", sourceDirectory, fullPath);
                    sourceDirectory = fullPath;
                }

                SourceDirectory = sourceDirectory;
            }

            public string SourceDirectory { get; }

            public override void Configure(MarkDocsOptions config)
            {
                base.Configure(config);

                config.FromStaticDirectory(SourceDirectory, watchForChanges: true);
            }
        }

        private sealed class GitServerConfig : ServerConfigBase
        {
            public GitServerConfig(
                string listenUrl,
                string publicUrl,
                string cacheDirectory,
                bool enableDeveloperMode,
                Theme theme,
                bool verbose,
                bool quiet,
                string fetchUrl,
                string username,
                string password,
                string[] branches,
                TimeSpan? pollInterval
                )
                : base($"{fetchUrl}#{branches[0]}", listenUrl, publicUrl, cacheDirectory, enableDeveloperMode, theme, verbose, quiet)
            {
                if (string.IsNullOrWhiteSpace(username) &&
                    !string.IsNullOrWhiteSpace(password))
                {
                    Console.Error.WriteLine(
                        $"{"WARNING!".Yellow()} \"--password\" has no effect unless \"--username\" is set too"
                    );
                }

                FetchUrl = fetchUrl;
                Username = username;
                Password = password;
                Branches = branches;
                PollInterval = pollInterval;
            }

            public string FetchUrl { get; }
            public string Username { get; }
            public string Password { get; }
            public string[] Branches { get; }
            public TimeSpan? PollInterval { get; }

            public override void Configure(MarkDocsOptions config)
            {
                base.Configure(config);

                var storageOptions = new GitSourceTreeOptions
                {
                    Directory = Path.Combine(CacheDirectory, "src"),
                    FetchUrl = FetchUrl,
                    UserName = Username,
                    Password = Password,
                    EnablePolling = PollInterval != null,
                    PollingInterval = PollInterval ?? Timeout.InfiniteTimeSpan,
                    Branches =
                    {
                        Include = Branches
                    }
                };
                config.FromGit(storageOptions);
            }
        }

        private class ValidationException : Exception
        {
            public ValidationException(string message)
                : base(message)
            { }
        }

        private static string ClampTrailingSlashes(string str)
        {
            if (str != null)
            {
                while (str.EndsWith("/"))
                {
                    str = str.Substring(0, str.Length - 1);
                }
            }

            return str;
        }
    }
}