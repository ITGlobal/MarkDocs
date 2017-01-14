using ITGlobal.MarkDocs;
using ITGlobal.MarkDocs.Comments.Data;
using ITGlobal.MarkDocs.Format;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     Parent reference for a comment
    /// </summary>
    internal interface ICommentParent
    {
        IPage Page { get; }
        ICommentDataRepository Repository { get; }
        IFormat Format { get; }
        void OnAdded(IComment comment);
        void OnDeleted(IComment comment);
    }
}