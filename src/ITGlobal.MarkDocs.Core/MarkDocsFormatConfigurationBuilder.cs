using System;
using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs
{
    /// <summary>
    ///     MarkDocs page format configuration builder
    /// </summary>
    [PublicAPI]
    public sealed class MarkDocsFormatConfigurationBuilder
    {
        private Func<MarkdocsFactoryContext, IFormat> _factory;
        
        /// <summary>
        ///     Applies specified configuration functions
        /// </summary>
        [PublicAPI, NotNull]
        public MarkDocsFormatConfigurationBuilder Use([NotNull] Func<MarkdocsFactoryContext, IFormat> func)
        {
            _factory = func;
            return this;
        }

        internal IFormat Build(MarkdocsFactoryContext context)
        {
            if (_factory == null)
            {
                throw new Exception($"Service {typeof(IFormat).FullName} is not configured");
            }

            return _factory(context);
        }
    }
}