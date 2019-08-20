using System;

namespace ITGlobal.MarkDocs.Tools
{
    public interface ITerminalOutput : IDisposable
    {
        void Write(int? value, string str = null);
    }
}