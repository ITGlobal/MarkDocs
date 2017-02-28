using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///    A context for <see cref="IFormat.Render"/>
    /// </summary>
    [PublicAPI]
    public interface IRenderContext
    {
        /// <summary>
        ///    A page reference
        /// </summary>
        [PublicAPI, NotNull]
        IPage Page { get; }

        /// <summary>
        ///   Add a generated attachment
        /// </summary>
        [PublicAPI, NotNull]
        IAttachment CreateAttachment([NotNull] string name, [NotNull] byte[] content);
    }
}