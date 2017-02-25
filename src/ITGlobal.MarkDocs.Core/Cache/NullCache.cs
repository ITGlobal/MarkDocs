using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     An non-caching implementation of <see cref="ICache"/>
    /// </summary>
    internal sealed class NullCache : ICache
    {
        #region fields

        private readonly object _contentProvidersLock = new object();
        private Dictionary<string, Dictionary<string, IResourceContent>> _contentProviders =
            new Dictionary<string, Dictionary<string, IResourceContent>>(StringComparer.OrdinalIgnoreCase);

        #endregion

        #region ICache

        /// <summary>
        ///     Starts update operation
        /// </summary>
        /// <returns>
        ///     Cache update operation
        /// </returns>
        ICacheUpdateOperation ICache.BeginUpdate() => new NullCacheUpdateOperation(this);

        /// <summary>
        ///     Gets a cached content
        /// </summary>
        /// <param name="item">
        ///     Cache item
        /// </param>
        /// <returns>
        ///     Cached content or null
        /// </returns>
        public Stream Read(IResource item)
        {
            var id = Path.Combine(item.Type.ToString(), item.Id);
           
            lock (_contentProvidersLock)
            {
                Dictionary<string, IResourceContent> dict;
                if (!_contentProviders.TryGetValue(item.Documentation.Id, out dict))
                {
                    return null;
                }

                IResourceContent provider;
                if (!dict.TryGetValue(id, out provider))
                {
                    return null;
                }

                return provider.GetContent();
            }
        }

        #endregion

        #region internal methods

        internal Dictionary<string, Dictionary<string, IResourceContent>> CloneContentProviders()
        {
            lock (_contentProvidersLock)
            {
                var clone = new Dictionary<string, Dictionary<string, IResourceContent>>(StringComparer.OrdinalIgnoreCase);
                foreach (var pair in _contentProviders)
                {
                    var dict = pair.Value.ToDictionary(_ => _.Key, _ => _.Value, StringComparer.OrdinalIgnoreCase);
                    clone[pair.Key] = dict;
                }

                return clone;
            }
        }

        internal void SetContentProviders(Dictionary<string, Dictionary<string, IResourceContent>> knownPages)
        {
            lock (_contentProvidersLock)
            {
                _contentProviders = knownPages;
            }
        }

        #endregion
    }
}