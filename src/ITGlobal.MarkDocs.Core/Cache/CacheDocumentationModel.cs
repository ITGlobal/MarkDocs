using ITGlobal.MarkDocs.Cache.Model;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Cache
{
    /// <summary>
    ///     A result for <see cref="ICacheProvider.Load"/>
    /// </summary>
    [PublicAPI]
    public sealed class CacheDocumentationModel
    {
        /// <summary>
        ///     .ctor
        /// </summary>
        public CacheDocumentationModel([NotNull] DocumentationModel model, [NotNull] ICacheReader cacheReader)
        {
            Model = model;
            CacheReader = cacheReader;
        }

        /// <summary>
        ///     Documentation model
        /// </summary>
        [NotNull]
        public DocumentationModel Model { get; }

        /// <summary>
        ///     A <see cref="ICacheReader"/> associated with current model
        /// </summary>
        [NotNull]
        public ICacheReader CacheReader { get; }

        /// <summary>
        ///     Deconstructor
        /// </summary>
        public void Deconstruct([NotNull] out DocumentationModel model, [NotNull] out ICacheReader cacheReader)
        {
            model = Model;
            cacheReader = CacheReader;
        }
    }
}