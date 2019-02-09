using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;
using ITGlobal.MarkDocs.Source.Impl;
using System.Collections.Generic;

namespace ITGlobal.MarkDocs.Impl
{
    internal sealed class CompilationReportBuilder : ICompilationReportBuilder
    {
        private readonly struct Msg
        {
            public Msg(CompilationReportMessageType type, string message, string path, int? lineNumber)
            {
                Type = type;
                Message = message;
                Path = path;
                LineNumber = lineNumber;
            }

            public readonly CompilationReportMessageType Type;
            public readonly string Message;
            public readonly string Path;
            public readonly int? LineNumber;

            public static Msg Warning(string message)
            {
                return new Msg(CompilationReportMessageType.Warning, message, null, null);
            }

            public static Msg Error(string message)
            {
                return new Msg(CompilationReportMessageType.Error, message, null, null);
            }

            public static Msg Warning(string message, string path, int? lineNumber)
            {
                return new Msg(CompilationReportMessageType.Warning, message, path, lineNumber);
            }

            public static Msg Error(string message, string path, int? lineNumber)
            {
                return new Msg(CompilationReportMessageType.Error, message, path, lineNumber);
            }

            public void Deconstruct(out CompilationReportMessageType type, out string message, out string path, out int? lineNumber)
            {
                type = Type;
                message = Message;
                path = Path;
                lineNumber = LineNumber;
            }
        }

        private readonly List<Msg> _messages = new List<Msg>();
        private readonly CompilationEventListener _listener;

        public CompilationReportBuilder(CompilationEventListener listener)
        {
            _listener = listener;
        }

        public void Error(string message)
        {
            _messages.Add(Msg.Error(message));
            _listener.Error(message);
        }

        public void Warning(string path, string message, int? lineNumber = null)
        {
            _messages.Add(Msg.Warning(message, path, lineNumber));
            _listener.Warning(path, message, lineNumber != null ? $":{lineNumber}" : null);
        }

        public void Error(string path, string message, int? lineNumber = null)
        {
            _messages.Add(Msg.Error(message, path, lineNumber));
            _listener.Error(path, message, lineNumber != null ? $":{lineNumber}" : null);
        }

        public CompilationReportModel Build(string rootDirectory)
        {
            var messages = new CompilationReportMessageModel[_messages.Count];
            for (var i = 0; i < _messages.Count; i++)
            {
                var (type, text, path, lineNumber) = _messages[i];
                if (path != null)
                {
                    path = PathHelper.GetRelativePath(rootDirectory, path);
                }

                messages[i] = new CompilationReportMessageModel
                {
                    Type = type,
                    Message = text,
                    Filename = path,
                    LineNumber = lineNumber
                };
            }

            return new CompilationReportModel
            {
                Messages = messages
            };
        }
    }
}