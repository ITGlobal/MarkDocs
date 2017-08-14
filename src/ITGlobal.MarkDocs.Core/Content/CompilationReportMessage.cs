using System;

namespace ITGlobal.MarkDocs.Content
{
    internal sealed class CompilationReportMessage : ICompilationReportMessage
    {
        private CompilationReportMessage(
            CompilationReportMessageType type,
            string message,
            int? lineNumber,
            Exception exception
        )
        {
            Type = type;
            Message = message;
            LineNumber = lineNumber;
            Exception = exception;
        }

        public CompilationReportMessageType Type { get; }

        public int? LineNumber { get; }

        public string Message { get; }

        public Exception Exception { get; }

        public static CompilationReportMessage Error(string message, int? lineNumber = null, Exception exception = null)
        {
            return new CompilationReportMessage(
                CompilationReportMessageType.Error,
                message,
                lineNumber,
                exception
            );
        }

        public static CompilationReportMessage Warning(string message, int? lineNumber = null, Exception exception = null)
        {
            return new CompilationReportMessage(
                CompilationReportMessageType.Warning,
                message,
                lineNumber,
                exception
            );
        }
    }
}