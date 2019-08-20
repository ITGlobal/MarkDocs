using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using ITGlobal.CommandLine.Table;
using Serilog;
using Serilog.Events;

namespace ITGlobal.MarkDocs.Site
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Env
    {
        static Env()
        {
            LISTEN_URL = EnvReader.Env(nameof(LISTEN_URL), "http://0.0.0.0:8000");
            LISTEN_URL = EnvReader.ClampTrailingSlashes(LISTEN_URL);

            PUBLIC_URL = EnvReader.Env(nameof(PUBLIC_URL));
            PUBLIC_URL = EnvReader.ClampTrailingSlashes(PUBLIC_URL);

            TEMP_DIR = EnvReader.Env(nameof(TEMP_DIR));
            ENABLE_DEV_MODE = EnvReader.Env(nameof(ENABLE_DEV_MODE), EnvReader.ParseBool);
            LOG_LEVEL = EnvReader.Env(nameof(LOG_LEVEL), EnvReader.ParseLogLevel, LogEventLevel.Debug);

            DOC_SOURCE_DIR = EnvReader.Env(nameof(DOC_SOURCE_DIR));
            DOC_GIT_FETCH_URL = EnvReader.Env(nameof(DOC_GIT_FETCH_URL));
            DOC_GIT_USERNAME = EnvReader.Env(nameof(DOC_GIT_USERNAME));
            DOC_GIT_PASSWORD = EnvReader.Env(nameof(DOC_GIT_PASSWORD));
            DOC_GIT_BRANCH = EnvReader.Env(nameof(DOC_GIT_BRANCH), EnvReader.ParseArray, new[] { "master" });
            DOC_GIT_POLL_INTERVAL = EnvReader.Env(nameof(DOC_GIT_POLL_INTERVAL), int.Parse, 5 * 60);
        }

        public static string LISTEN_URL { get; }
        public static string PUBLIC_URL { get; private set; }
        public static string TEMP_DIR { get; private set; }
        public static bool ENABLE_DEV_MODE { get; }
        public static LogEventLevel LOG_LEVEL { get; }
        public static string DOC_SOURCE_DIR { get; private set; }
        public static string DOC_GIT_FETCH_URL { get; }
        public static string DOC_GIT_USERNAME { get; }
        public static string DOC_GIT_PASSWORD { get; }
        public static string[] DOC_GIT_BRANCH { get; }
        public static int DOC_GIT_POLL_INTERVAL { get; }

        public static void Initialize()
        {
            Log.Information(@"Configuration:");
            var table = TerminalTable.CreateFluent(TableRenderer.Grid());
            table.Title("Configuration");
            table.Add("LISTEN_URL", LISTEN_URL);
            table.Add("PUBLIC_URL", PUBLIC_URL);
            table.Add("TEMP_DIR", TEMP_DIR);
            table.Add("ENABLE_DEV_MODE", ENABLE_DEV_MODE ? "YES" : "NO");
            table.Add("LOG_LEVEL", LOG_LEVEL.ToString());
            table.Add("DOC_SOURCE_DIR", DOC_SOURCE_DIR);
            table.Add("DOC_GIT_FETCH_URL", DOC_GIT_FETCH_URL);
            table.Add("DOC_GIT_USERNAME", DOC_GIT_USERNAME);
            table.Add("DOC_GIT_PASSWORD", new string('*', DOC_GIT_PASSWORD?.Length ?? 0));
            table.Add("DOC_GIT_BRANCH", string.Join("\n", DOC_GIT_BRANCH));
            table.Add("DOC_GIT_POLL_INTERVAL", $"{DOC_GIT_POLL_INTERVAL} sec");
            table.Draw(s => Log.Information("{S}", s));

            if (!Uri.TryCreate(LISTEN_URL, UriKind.Absolute, out var u) ||
                u.Scheme != Uri.UriSchemeHttp ||
                u.PathAndQuery != "/")
            {
                Log.Error("${Env} is not set or is malformed", nameof(LISTEN_URL));
                Environment.Exit(-1);
            }

            if (string.IsNullOrWhiteSpace(PUBLIC_URL))
            {
                var url = new UriBuilder(LISTEN_URL);
                url.Host = "localhost";
                PUBLIC_URL = url.ToString();
                PUBLIC_URL = EnvReader.ClampTrailingSlashes(PUBLIC_URL);

                Log.Warning("${Env} is not set. Falling back to \"{Fallback}\"", nameof(PUBLIC_URL), PUBLIC_URL);
            }

            if (string.IsNullOrWhiteSpace(TEMP_DIR))
            {
                TEMP_DIR = Path.Combine(Path.GetTempPath(), "markdocs");
                Log.Warning("${Env} is not set. Falling back to \"{Fallback}\"", nameof(TEMP_DIR), TEMP_DIR);
            }

            if (string.IsNullOrWhiteSpace(DOC_SOURCE_DIR) &&
                string.IsNullOrWhiteSpace(DOC_GIT_FETCH_URL))
            {
                Log.Error("Neither ${E1} nor ${E2} are set", nameof(DOC_SOURCE_DIR), nameof(DOC_GIT_FETCH_URL));
                Environment.Exit(-1);
            }

            if (!string.IsNullOrWhiteSpace(DOC_GIT_FETCH_URL))
            {
                if (!Uri.TryCreate(DOC_GIT_FETCH_URL, UriKind.Absolute, out u) ||
                    (u.Scheme != Uri.UriSchemeHttp && u.Scheme != "git"))
                {
                    Log.Error("${Env} is malformed", nameof(DOC_GIT_FETCH_URL));
                    Environment.Exit(-1);
                }

                if (string.IsNullOrWhiteSpace(DOC_GIT_USERNAME) &&
                    !string.IsNullOrWhiteSpace(DOC_GIT_PASSWORD))
                {
                    Log.Warning("WARNING! ${E1} has no effect unless ${E2} is set", nameof(DOC_GIT_PASSWORD), nameof(DOC_GIT_USERNAME));
                }
            }

            if (!string.IsNullOrWhiteSpace(DOC_SOURCE_DIR))
            {
                var fullPath = Path.GetFullPath(DOC_SOURCE_DIR);
                if (DOC_SOURCE_DIR != fullPath)
                {
                    Log.Debug("Source path \"{Path}\" resolved as \"{FullPath}\"", DOC_SOURCE_DIR, fullPath);
                    DOC_SOURCE_DIR = fullPath;
                }
            }
        }
    }
}