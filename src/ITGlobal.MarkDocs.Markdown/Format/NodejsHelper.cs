using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ITGlobal.MarkDocs.Format
{
    internal sealed class NodejsHelper
    {
        private const string EXECUTABLE_NAME = "node";
        private readonly ILogger _log;

        public NodejsHelper(ILogger log)
        {
            _log = log;
        }

        public bool IsNodeInstalled
        {
            get
            {
                try
                {
                    using (var exec = new ExecuteHelper(_log, EXECUTABLE_NAME, verboseOutput: false))
                    {
                        exec.Run(new[] { "--version" });
                        exec.ThrowIfFailed();

                        var version = exec.StandardOutput.FirstOrDefault();
                        _log.LogDebug("nodejs is installed: {0}", version);

                        return true;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public T Invoke<T>(string module, object args = null)
        {
            var directory = Path.GetDirectoryName(module);
            using (var exec = new ExecuteHelper(_log, EXECUTABLE_NAME, directory))
            {
                var input = args != null
                    ? JsonConvert.SerializeObject(args, Formatting.None)
                    : null;
                exec.Run(new[] {module}, input != null ? new[] {input, "\u001A" /* EOF mark */} : null);
                exec.ThrowIfFailed();

                var output = string.Join("\n", exec.StandardOutput);
                var result = JsonConvert.DeserializeObject<T>(output);
                return result;
            }
        }
    }
}