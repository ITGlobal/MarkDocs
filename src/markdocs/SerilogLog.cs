using System;
using Serilog;

namespace ITGlobal.MarkDocs.Tools
{
    public sealed class SerilogLog : IMarkDocsLog
    {
        public void Debug(string message)
        {
            Log.Debug("{M}", message);
        }

        public void Info(string message)
        {
            Log.Information("{M}", message);
        }

        public void Warning(string message)
        {
            Log.Warning("{M}", message);
        }

        public void Error(string message)
        {
            Log.Error("{M}", message);
        }

        public void Error(Exception e, string message)
        {
            Log.Error(e, "{M}", message);
        }
    }
}