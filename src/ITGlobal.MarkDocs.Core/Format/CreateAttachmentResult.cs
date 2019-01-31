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
        private readonly Func<GeneratedAsset, Stream> _readFunc;

        /// <summary>
        ///     .ctor
        /// </summary>
        public CreateAttachmentResult([NotNull] GeneratedAsset asset, [NotNull] Func<GeneratedAsset, Stream> readFunc)
        {
            Asset = asset;
            _readFunc = readFunc;
        }

        /// <summary>
        ///     A generated asset
        /// </summary>
        [NotNull]
        public GeneratedAsset Asset { get; }

        /// <summary>
        ///     Opens asset content for reading
        /// </summary>
        [NotNull]
        public Stream OpenRead() => _readFunc(Asset);
    }
}