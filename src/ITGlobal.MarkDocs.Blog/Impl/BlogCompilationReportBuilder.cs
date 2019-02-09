using System.Collections.Generic;
using System.Linq;
using ITGlobal.MarkDocs.Cache.Model;
using ITGlobal.MarkDocs.Source;
using ITGlobal.MarkDocs.Source.Impl;

namespace ITGlobal.MarkDocs.Blog.Impl
{
    internal sealed class BlogCompilationReportBuilder : ICompilationReportBuilder
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
       
        public void Error(string message)
        {
            _messages.Add(Msg.Error(message));
        }

        public void Warning(string path, string message, int? lineNumber = null)
        {
            _messages.Add(Msg.Warning(message, path, lineNumber));
        }

        public void Error(string path, string message, int? lineNumber = null)
        {
            _messages.Add(Msg.Error(message, path, lineNumber));
        }

        public CompilationReportModel Build(string rootDirectory)
        {
            var messages = new CompilationReportMessageModel[_messages.Count];
            for (int i = 0; i < _messages.Count; i++)
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

        public CompilationReportModel Merge(ICompilationReport report)
        {
            return new CompilationReportModel
            {
                Messages = Iterator().ToArray()
            };

            IEnumerable<CompilationReportMessageModel> Iterator()
            {
                foreach (var (path, messages) in report.Messages)
                {
                    foreach (var message in messages)
                    {
                        yield return new CompilationReportMessageModel
                        {
                            Type = message.Type,
                            Message = message.Message,
                            Filename = path,
                            LineNumber = message.LineNumber
                        };
                    }
                }

                foreach (var (type, text, path, lineNumber) in _messages)
                {
                    yield return new CompilationReportMessageModel
                    {
                        Type = type,
                        Message = text,
                        Filename = path,
                        LineNumber = lineNumber
                    };
                }
            }
        }
    }
}