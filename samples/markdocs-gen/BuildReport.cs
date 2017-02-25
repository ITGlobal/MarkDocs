using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;
using static ITGlobal.CommandLine.CLI;

namespace ITGlobal.MarkDocs.StaticGen
{
    public sealed class BuildReport
    {
        private abstract class BuildReportItem
        {
            public abstract void Print();
        }

        private sealed class WarningBuildReportItem : BuildReportItem
        {
            private readonly string _message;
            private readonly string _location;
            
            public WarningBuildReportItem(string message, string location)
            {
                _message = message;
                _location = location;
            }

            public override void Print()
            {
                using (WithForeground(ConsoleColor.Yellow))
                {
                    Write("\tWARN:\t");
                }

                if (!string.IsNullOrWhiteSpace(_location))
                {
                    Write("at {0}:\t", _location);
                }


                WriteLine(_message);
            }
        }

        private sealed class ErrorBuildReportItem : BuildReportItem
        {
            private readonly string _message;

            public ErrorBuildReportItem(string message)
            {
                _message = message;
            }

            public override void Print()
            {
                using (WithForeground(ConsoleColor.Yellow))
                {
                    Write("\tERROR:\t");
                }

                WriteLine(_message);
            }
        }

        private readonly Dictionary<string, List<BuildReportItem>> _items = new Dictionary<string, List<BuildReportItem>>();
        private int _errorCount;
        private int _warningCount;

        public void AddWarning(string pageId, string message, string location)
        {
            Add(pageId, new WarningBuildReportItem(message, location));
            _warningCount++;
        }

        public void AddError(string pageId, string message)
        {
            Add(pageId, new ErrorBuildReportItem(message));
            _errorCount++;
        }

        public void Print()
        {
            using (WithForeground(ConsoleColor.Cyan))
            {
                WriteLine();
                WriteLine("Build report");
                WriteLine("============");
                WriteLine();
            }
            
            foreach (var pageId in _items.Keys.OrderBy(_=>_))
            {
                WriteLine("PAGE {0}:", pageId);
                foreach (var item in _items[pageId])
                {
                    item.Print();
                }
            }

            WriteLine();
            if (_errorCount == 0)
            {
                using (WithForeground(ConsoleColor.Green))
                {
                    WriteLine("Compilation succeeded.");
                }
            }
            else
            {
                using (WithForeground(ConsoleColor.Red))
                {
                    WriteLine("Compilation failed.");
                }
            }

            WriteLine("\t{0} warning(s)", _warningCount);
            WriteLine("\t{0} errors(s)", _errorCount);
            WriteLine();
        }

        public void Clear()
        {
            _items.Clear();
            _warningCount = 0;
            _errorCount = 0;
        }

        private void Add(string pageId, BuildReportItem item)
        {
            List<BuildReportItem> list;
            if (!_items.TryGetValue(pageId, out list))
            {
                list = new List<BuildReportItem>();
                _items.Add(pageId, list);
            }

            list.Add(item);
        }
    }
}