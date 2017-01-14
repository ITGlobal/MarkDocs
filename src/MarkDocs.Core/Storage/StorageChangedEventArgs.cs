using System;
using ITGlobal.MarkDocs.Content;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Storage
{
    /// <summary>
    ///     Arguments for <see cref="IStorage.Changed"/> event
    /// </summary>
    [PublicAPI]
    public sealed class StorageChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        [PublicAPI]
        public StorageChangedEventArgs(string documentationId = null)
        {
            if (!string.IsNullOrEmpty(documentationId))
            {
                Documentation.NormalizeId(ref documentationId);
            }

            DocumentationId = documentationId;
        }

        /// <summary>
        ///     Documentation ID if only one documentation has been changed.
        ///     null if all documentations have been updated.
        /// </summary>
        [PublicAPI]
        [CanBeNull]
        public string DocumentationId { get; }
    }
}