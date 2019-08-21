using System;
using ITGlobal.CommandLine;

namespace ITGlobal.MarkDocs.Tools
{
    public static class TerminalOutput
    {
        public static ITerminalOutput Create(string title, bool quiet = false)
        {
            if (quiet)
            {
                return new QuietTerminalOutput();
            }

            if (Console.IsOutputRedirected)
            {
                return new RedirectedTerminalOutput(title);
            }

            return new NormalTerminalOutput(title);
        }

        private sealed class QuietTerminalOutput : ITerminalOutput
        {
            public void Write(int? value, string str = null) { }

            public void Dispose() { }
        }

        private sealed class RedirectedTerminalOutput : ITerminalOutput
        {
            private int _maxWidth;
            private int _value;
            private string _str;

            public RedirectedTerminalOutput(string str)
            {
                Write(0, str);
            }
            
            public void Write(int? value, string str = null)
            {
                if (value != null && value.Value != _value ||
                    str != null && str != _str)
                {
                    _value = value ?? _value;
                    _str = str ?? _str;


                    var s = $"{_str} ({_value}%)";
                    _maxWidth = Math.Max(_maxWidth, s.Length);

                    s = s.PadRight(_maxWidth);
                    Console.Error.Write("\r");
                    Console.Error.Write(s);
                }
            }

            public void Dispose()
            {
                Console.Error.Write("\r");
                Console.Error.Write("".PadRight(_maxWidth));
                Console.Error.Write("\r");
            }
        }

        private sealed class NormalTerminalOutput : ITerminalOutput
        {
            private readonly ILiveOutputManager _manager;
            private readonly ITerminalLiveProgressBar _progressBar;

            private int _value;
            private string _str;

            public NormalTerminalOutput(string str)
            {
                _str = str;
                _manager = LiveOutputManager.Create(progressBarRenderer: ProgressBarRenderer.HashSign());
                _progressBar = _manager.CreateProgressBar(str);
                _progressBar.WipeAfter();
            }

            public void Write(int? value, string str = null)
            {
                if (value != null && value.Value != _value ||
                   str != null && str != _str)
                {
                    _value = value ?? _value;
                    _str = str ?? _str;

                    _progressBar.Write(_value, _str);
                }
            }

            public void Dispose() => _manager.Dispose();
        }
    }
}