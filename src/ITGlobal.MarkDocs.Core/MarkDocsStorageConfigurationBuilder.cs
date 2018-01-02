using System;
using ITGlobal.MarkDocs.Storage;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs storage configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsStorageConfigurationBuilder
    {
        private Func<MarkdocsFactoryContext, IStorage> _factory;
        
        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsStorageConfigurationBuilder Use([NotNull] Func<MarkdocsFactoryContext, IStorage> func)
        {
            _factory = func;
            return this;
        }
        
        internal IStorage Build(MarkdocsFactoryContext context)
        {
            if (_factory == null)
            {
                throw new Exception($"Service {typeof(IStorage).FullName} is not configured");
            }

            return _factory(context);
        }
    }
}