using System;
using ITGlobal.CommandLine;

namespace ITGlobal.MarkDocs.Tools
{
    public static class CliHelper
    {
        public static IDisposable SpinnerSafe(string title)
        {
            if (!Console.IsOutputRedirected)
            {
                return CLI.Spinner(title);
            }

            Console.Error.WriteLine(title);
            return null;
        }
    }
}