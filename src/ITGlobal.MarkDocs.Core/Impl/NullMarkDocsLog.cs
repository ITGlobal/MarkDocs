using System;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class NullMarkDocsLog : IMarkDocsLog
    {
        public void Debug(string message) { }

        public void Info(string message) { }

        public void Warning(string message) { }

        public void Error(string message) { }

        public void Error(Exception e, string message) { }
    }
}