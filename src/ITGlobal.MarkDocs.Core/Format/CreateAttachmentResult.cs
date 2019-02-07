using System;
using System.IO;
using ITGlobal.MarkDocs.Source;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Format
{
    /// <summary>
    ///     Generated asset write result
    /// </summary>
    [PublicAPI]
    public readonly struct CreateAttachmentResult
    {
        private readonly Func<GeneratedFileAsset, Stream> _readFunc;

        /// <summary>
        ///     .ctor
        /// </summary>
        public CreateAttachmentResult([NotNull] GeneratedFileAsset asset, [NotNull] Func<GeneratedFileAsset, Stream> readFunc)
        {
            Asset = asset;
            _readFunc = readFunc;
        }

        /// <summary>
        ///     A generated asset
        /// </summary>
        [NotNull]
        public GeneratedFileAsset Asset { get; }

        /// <summary>
        ///     Opens asset content for reading
        /// </summary>
        [NotNull]
        public Stream OpenRead() => _readFunc(Asset);
    }
}