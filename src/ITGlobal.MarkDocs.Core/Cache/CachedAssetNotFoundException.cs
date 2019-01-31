using System;
using System.Runtime.Serialization;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     Thrown when a cached content is not found
    /// </summary>
    [Serializable]
    public class CachedAssetNotFoundException : Exception
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public CachedAssetNotFoundException()
        {
        }

        /// <summary>
        ///     .ctor
        /// </summary>
        public CachedAssetNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        ///     .ctor
        /// </summary>
        public CachedAssetNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        ///     .ctor
        /// </summary>
        protected CachedAssetNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}