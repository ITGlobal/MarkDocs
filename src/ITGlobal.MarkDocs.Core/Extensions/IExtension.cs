using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Extensions
{
    /// <summary>
    ///     MarkDocs extension
    /// </summary>
    [PublicAPI]
    public interface IExtension
    {
        /// <summary>
        ///     Initialize extension.
        ///     This method is called at startup, no more than once.
        ///     If there's no cached documentation - this method won't be called at all.
        /// </summary>
        void Initialize([NotNull] IMarkDocState state);

        /// <summary>
        ///     Called when new documentation branch is added 
        /// </summary>
        void OnCreated([NotNull] IDocumentation documentation);

        /// <summary>
        ///     Called when a documentation branch is updated
        /// </summary>
        void OnUpdated([NotNull] IDocumentation documentation);

        /// <summary>
        ///     Called when a documentation branch is updated and all changes are committed
        /// </summary>
        void OnUpdateCompleted([NotNull] IDocumentation documentation);

        /// <summary>
        ///     Called when a documentation branch is removed
        /// </summary>
        void OnRemoved([NotNull] IDocumentation documentation);
    }
}