using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     Markdocs service creation context
    /// </summary>
    public sealed class MarkdocsFactoryContext
    {
        internal MarkdocsFactoryContext(
            [NotNull] ILoggerFactory loggerFactory,
            [NotNull] IMarkDocsEventCallback callback)
        {
            LoggerFactory = loggerFactory;
            Callback = callback;
        }

        /// <summary>
        ///     Logger factory
        /// </summary>
        [NotNull]
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>
        ///     Event callback
        /// </summary>
        [NotNull]
        public IMarkDocsEventCallback Callback { get; }
    }
}