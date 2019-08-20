using System;
using ITGlobal.CommandLine;

namespace ITGlobal.MarkDocs.Tools
{
    public static class CliHelper
    {
        private sealed class SpinnerWrapper : IDisposable
        {
            private readonly ILiveOutputManager _manager;

            public SpinnerWrapper(string title)
            {
                _manager = LiveOutputManager.Create();
                _manager.CreateSpinner(title).WipeAfter();
            }

            public void Dispose()
            {
                _manager.Dispose();
            }
        }

        public static IDisposable SpinnerSafe(string title)
        {
            if (!Console.IsOutputRedirected)
            {
                return new SpinnerWrapper(title);
            }

            Console.WriteLine(title);
            return null;
        }
    }
}