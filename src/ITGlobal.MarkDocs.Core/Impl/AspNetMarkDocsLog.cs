using System;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class AspNetMarkDocsLog : IMarkDocsLog
    {
        private readonly ILogger _log;

        public AspNetMarkDocsLog(ILoggerFactory loggerFactory)
        {
            _log = loggerFactory.CreateLogger(typeof(IMarkDocService).Namespace);
        }

        public void Debug(string message) => _log.LogDebug(message);

        public void Info(string message) => _log.LogInformation(message);

        public void Warning(string message) => _log.LogWarning(message);

        public void Error(string message) => _log.LogError(message);

        public void Error(Exception e, string message) => _log.LogError(e, message);
    }
}