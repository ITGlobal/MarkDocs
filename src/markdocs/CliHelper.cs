using System;
using ITGlobal.CommandLine;
using ITGlobal.CommandLine.Spinners;

namespace ITGlobal.MarkDocs.Tools
{
    public static class CliHelper
    {
        public static IDisposable SpinnerSafe(string title)
        {
            if (!Console.IsOutputRedirected)
            {
                return TerminalSpinner.Create(title);
            }

            Terminal.Stderr.WriteLine(title);
            return null;
        }
    }
}