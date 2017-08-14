using System;

namespace ITGlobal.MarkDocs.Format
{
    internal sealed class FakeRenderContext : IRenderContext
    {
        private readonly IParseContext _ctx;

        public FakeRenderContext(IParseContext ctx)
        {
            _ctx = ctx;
        }

        public IPage Page => _ctx.Page;

        public IAttachment CreateAttachment(string name, byte[] content)
        {
            throw new NotSupportedException();
        }

        public void Warning(string message, int? lineNumber = null, Exception exception = null)
        {
            _ctx.Warning(message, lineNumber, exception);
        }

        public void Error(string message, int? lineNumber = null, Exception exception = null)
        {
            _ctx.Error(message, lineNumber, exception);
        }
    }
}