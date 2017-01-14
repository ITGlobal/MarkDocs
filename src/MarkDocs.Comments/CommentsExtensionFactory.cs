using ITGlobal.MarkDocs;
using ITGlobal.MarkDocs.Comments.Data;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Format;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     A factory for <see cref="CommentsExtension"/>
    /// </summary>
    internal sealed class CommentsExtensionFactory : IExtensionFactory
    {
        private readonly ICommentDataRepository _repository;
        private readonly IFormat _format;
        private readonly ILoggerFactory _loggerFactory;

        public CommentsExtensionFactory(ICommentDataRepository repository, IFormat format, ILoggerFactory loggerFactory)
        {
            _repository = repository;
            _format = format;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        ///     Create an instance of an extension
        /// </summary>
        /// <param name="service">
        ///     MarkDocs service
        /// </param>
        /// <param name="state">
        ///     Initial documentation state
        /// </param>
        /// <returns>
        ///     MarkDocs extension
        /// </returns>
        public IExtension Create(IMarkDocService service, IMarkDocServiceState state)
            => new CommentsExtension(_repository, _format, _loggerFactory, state);
    }
}