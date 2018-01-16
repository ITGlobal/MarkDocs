using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs
{
    internal sealed class ExecuteHelper : IDisposable
    {
        private enum OutputStream
        {
            Stdin = 0,
            Stdout = 1,
            Stderr = 2
        }

        private struct OutputLine
        {
            public OutputLine(OutputStream stream, string line)
            {
                Stream = stream;
                Line = line;
            }

            public OutputStream Stream { get; }
            public string Line { get; }
        }

        private readonly object _outputLock = new object();
        private readonly List<OutputLine> _output = new List<OutputLine>();

        private readonly ILogger _log;
        private readonly string _programName;
        private readonly Process _process;
        private readonly bool _verboseOutput;

        public ExecuteHelper(ILogger log, string programName, string workDirectory = null, bool verboseOutput = false)
        {
            _log = log;
            _programName = programName;
            _verboseOutput = verboseOutput;

            _process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    FileName = programName,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            if (!string.IsNullOrEmpty(workDirectory))
            {
                _process.StartInfo.WorkingDirectory = workDirectory;
            }

            _process.OutputDataReceived += (_, e) => WriteStandardOutput(e.Data);
            _process.ErrorDataReceived += (_, e) => WriteStandardError(e.Data);
        }

        public IEnumerable<string> StandardOutput
            => _output.Where(_ => _.Stream == OutputStream.Stdout).Select(_ => _.Line);
        public IEnumerable<string> StandardError
            => _output.Where(_ => _.Stream == OutputStream.Stderr).Select(_ => _.Line);
        public int ExitCode { get; private set; }

        public void Run(IList<string> arguments, IList<string> stdin = null)
        {
            _process.StartInfo.Arguments = string.Join(" ", from arg in arguments select EscapeArgument(arg));
            _process.StartInfo.RedirectStandardInput = stdin != null;

            var w = Stopwatch.StartNew();
            _process.Start();

            _log.LogDebug($"{_programName} {_process.StartInfo.Arguments} (PID {_process.Id})");

            _process.BeginErrorReadLine();
            _process.BeginOutputReadLine();

            if (stdin != null)
            {
                foreach (var line in stdin)
                {
                    var buffer = Encoding.UTF8.GetBytes(line);
                    _process.StandardInput.BaseStream.Write(buffer, 0, buffer.Length);
                    WriteStandardInput(line);
                }
                _process.StandardInput.Flush();
            }

            _process.WaitForExit();

            ExitCode = _process.ExitCode;
            w.Stop();

            _log.LogDebug($"{_programName} exited with {_process.ExitCode} in {w.ElapsedMilliseconds}ms");
        }

        public void ThrowIfFailed()
        {
            if (ExitCode != 0)
            {
                var message = $"Command '{_programName} {_process.StartInfo.Arguments}' exited with {ExitCode}";
                throw new Exception(message);
            }
        }

        private string EscapeArgument(string argument)
        {
            if (argument.All(c => char.IsLetterOrDigit(c) || char.IsPunctuation(c)))
            {
                return argument;
            }

            return "\"" + argument + "\"";
        }

        public void Dispose() => _process.Dispose();

        private void WriteStandardInput(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                lock (_outputLock)
                {
                    _output.Add(new OutputLine(OutputStream.Stdin, line));
                }

                _log.LogDebug("\t< {0}", line);
            }
        }

        private void WriteStandardOutput(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                lock (_outputLock)
                {
                    _output.Add(new OutputLine(OutputStream.Stdout, line));
                }

                _log.LogDebug("\t1> {0}", line);
            }
        }

        private void WriteStandardError(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                lock (_outputLock)
                {
                    _output.Add(new OutputLine(OutputStream.Stderr, line));
                }

                _log.LogDebug("\t1> {0}", line);
            }
        }
    }
}