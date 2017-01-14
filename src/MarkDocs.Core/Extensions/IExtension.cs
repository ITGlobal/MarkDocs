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
        ///     Handle a documentation state update
        /// </summary>
        /// <param name="state">
        ///     New documentation state
        /// </param>
        [PublicAPI]
        void Update([NotNull] IMarkDocServiceState state);
    }
}