using System;
using ITGlobal.CommandLine;

namespace ITGlobal.MarkDocs.StaticGen
{
    public sealed class EventCallback : MarkDocsEventCallback
    {
        private sealed class WarningInfo
        {
            public string PageId { get; set; }
            public string Message { get; set; }
            public string Location { get; set; }
        }

        private sealed class ErrorInfo
        {
            public string PageId { get; set; }
            public string Message { get; set; }
        }

        private readonly BuildReport report = new BuildReport();
        private IProgressBar progressBar;

        public override void CompilationStarted(string id)
        {
            progressBar = CLI.ProgressBar();
            progressBar.SetState(0, "compiling");
        }

        public override void CompilingPage(string documentationId, string id, int i, int count)
        {
            var progress = (int)Math.Ceiling(100f * i / count);
            progressBar?.SetState(progress);
        }

        public override void Error(string documentationId, string pageId, Exception e)
        {
            report.AddError(pageId, e.Message);
        }

        public override void Warning(string documentationId, string pageId, string message, string location = null)
        {
            report.AddWarning(pageId, message, location ?? "");
        }

        public override void CompilationCompleted(string id)
        {
            progressBar?.Dispose();
            progressBar = null;

            report.Print();
            report.Clear();
        }
    }
}