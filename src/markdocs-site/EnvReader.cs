using System;
using System.Linq;
using Serilog;
using Serilog.Events;

namespace ITGlobal.MarkDocs.Site
{
    public static class EnvReader
    {
        public static string Env(string name, string defaultValue = "")
        {
            var value = Environment.GetEnvironmentVariable(name);
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return value;
        }

        public static T Env<T>(string name, Func<string, T> parser, T defaultValue = default)
        {
            var str = Env(name);
            if (string.IsNullOrEmpty(str))
            {
                return defaultValue;
            }

            try
            {
                return parser(str);
            }
            catch (Exception e)
            {
                Log.Error("Env variable ${Name} cannot be parsed: \"{Str}\"", name, str);
                Log.Error("{Msg}", e.Message);
                throw;
            }
        }

        public static string ClampTrailingSlashes(string str)
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

        public static string[] ParseArray(string str)
        {
            return (str ?? "").Split(';')
                .Select(_ => _.Trim())
                .Where(_ => !string.IsNullOrWhiteSpace(_))
                .ToArray();
        }

        public static bool ParseBool(string str)
        {
            switch ((str ?? "").ToLowerInvariant())
            {
                case "1":
                case "y":
                case "true":
                    return true;
                default:
                    return false;
            }
        }

        internal static LogEventLevel ParseLogLevel(string str)
        {
            switch ((str ?? "").ToUpperInvariant())
            {
                case "V":
                case "VRB":
                case "VERBOSE":
                case "DIAG":
                case "T":
                case "TRC":
                case "TRACE":
                    return LogEventLevel.Verbose;

                case "D":
                case "DEBUG":
                case "DBG":
                    return LogEventLevel.Debug;

                case "I":
                case "INF":
                case "INFO":
                case "INFORM":
                case "INFORMATION":
                    return LogEventLevel.Information;

                case "W":
                case "WRN":
                case "WARN":
                case "WARNING":
                    return LogEventLevel.Warning;

                case "E":
                case "ERR":
                case "ERROR":
                    return LogEventLevel.Error;

                default:
                    throw new Exception($"Invalid log level: \"{str}\"");
            }
        }
    }
}