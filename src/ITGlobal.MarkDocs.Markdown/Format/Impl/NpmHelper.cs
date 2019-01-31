using System;
using System.Linq;
using System.Runtime.InteropServices;
using ITGlobal.MarkDocs.Impl;

namespace ITGlobal.MarkDocs.Format.Impl
{
    internal sealed class NpmHelper
    {
        private readonly IMarkDocsLog _log;
        private readonly string _npmPath;

        public NpmHelper(IMarkDocsLog log)
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
                    using (var exec = new ExecuteHelper(_log, _npmPath, verboseOutput: false))
                    {
                        exec.Run(new[] { "--version" });
                        exec.ThrowIfFailed();

                        var version = exec.StandardOutput.FirstOrDefault();
                        _log.Debug($"NPM is installed: {version}");

                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public void Install(string directory)
        {
            // $ npm install {package}

            using (var exec = new ExecuteHelper(_log, _npmPath, directory))
            {
                exec.Run(new[] { "install" });
                exec.ThrowIfFailed();
            }
        }

        public void Install(string directory, string package)
        {
            // $ npm install {package}

            using (var exec = new ExecuteHelper(_log, _npmPath, directory))
            {
                exec.Run(new[] { "install", package });
                exec.ThrowIfFailed();
            }
        }

        private static string DetectNpmLocation(IMarkDocsLog log)
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

            using (var exec = new ExecuteHelper(log, command))
            {
                exec.Run(new[] { arg });
                exec.ThrowIfFailed();

                var location = exec.StandardOutput.First(s => !string.IsNullOrWhiteSpace(s));
                log.Debug($"NPM located at path {location}");
                return location;
            }
        }
    }
}