using System;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Format
{
    internal sealed class NpmHelper
    {
        private readonly ILogger _log;
        private readonly string _npmPath;

        public NpmHelper(ILogger log)
        {
            _log = log;

            _npmPath = DetectNpmLocation(log);
        }
        
        public bool IsNpmInstalled
        {
            get
            {
                try
                {
                    using (var exec = new ExecuteHelper(_log, _npmPath))
                    {
                        exec.Run(new[] { "--version" });
                        exec.ThrowIfFailed();

                        var version = exec.StandardOutput.FirstOrDefault();
                        _log.LogDebug("NPM is installed: {0}", version);

                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
        
        public void Install(string directory, string package)
        {
            // $ npm install {package}

            using (var exec = new ExecuteHelper(_log, _npmPath, directory, true))
            {
                exec.Run(new[] { "install", package });
                exec.ThrowIfFailed();
            }
        }

        private static string DetectNpmLocation(ILogger log)
        {
            string command, arg;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                command = "where";
                arg = "npm.cmd";
            }
            else
            {
                command = "which";
                arg = "npm";
            }

            using (var exec = new ExecuteHelper(log, command, null, true))
            {
                exec.Run(new[] { arg });
                exec.ThrowIfFailed();

                var location = exec.StandardOutput.First(s => !string.IsNullOrWhiteSpace(s));
                log.LogDebug("NPM located at path {0}", location);
                return location;
            }
        }
    }
}