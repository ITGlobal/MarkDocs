using System;
using ITGlobal.CommandLine.ProgressBars;

namespace ITGlobal.MarkDocs.Tools.Generate
{
    public sealed class GeneratorCallback : MarkDocsEventCallback
    {
        private IProgressBar _progressBar;

        private sealed class DisposableToken : IDisposable
        {
            private readonly Action _action;

            public DisposableToken(Action action)
            {
                _action = action;
            }

            public void Dispose() => _action();
        }

        public override IDisposable CompilationStarted(string id)
        {
            _progressBar = TerminalProgressBar.Create();
            _progressBar.SetState(0, "compiling");

            return new DisposableToken(() =>
            {
                _progressBar?.Dispose();
                _progressBar = null;
            });
        }

        public override void RenderedPage(string documentationId, string id, int i, int count)
        {
            var progress = (int)Math.Ceiling(100f * i / count);
            _progressBar?.SetState(progress);
        }
    }
}