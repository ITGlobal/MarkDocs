using System;
using ITGlobal.MarkDocs.Cache;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs content cache configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsCacheConfigurationBuilder
    {
        private Func<MarkdocsFactoryContext, ICache> _factory;

        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsCacheConfigurationBuilder Use([NotNull] Func<MarkdocsFactoryContext, ICache> func)
        {
            _factory = func;
            return this;
        }

        internal ICache Build(MarkdocsFactoryContext context)
        {
            if (_factory == null)
            {
                throw new Exception($"Service {typeof(ICache).FullName} is not configured");
            }

            return _factory(context);
        }
    }
}