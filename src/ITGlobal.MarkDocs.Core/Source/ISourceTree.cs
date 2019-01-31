using System;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Source
{
    /// <summary>
    ///     A single documentation source tree
    /// </summary>
    [PublicAPI]
    public interface ISourceTree
    {
        /// <summary>
        ///     Documentation branch ID
        /// </summary>
        [NotNull]
        string Id { get; }

        /// <summary>
        ///     This event is raised when documentation source is changed.
        ///     This event is raised only if storage supports change tracking.
        /// </summary>
        event EventHandler Changed;

        /// <summary>
        ///     Reads source tree and parses them into an asset tree
        /// </summary>
        [NotNull]
        AssetTree ReadAssetTree([NotNull] ICompilationReportBuilder report);

        /// <summary>
        ///     Refreshes this source tree
        /// </summary>
        void Refresh();
    }
}