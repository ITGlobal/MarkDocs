using System.Threading.Tasks;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Search;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Search
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsExtensionConfigurationBuilder"/> to configure tags extension
    /// </summary>
    [PublicAPI]
    public static class MarkdocsConfigurationExtensions
    {
        /// <summary>
        ///     Adds tags support into MarkDocs
        /// </summary>
        /// <param name="configuration">
        ///     Configuration builder
        /// </param>
        /// <param name="indexDirectory">
        ///     Path to lucene index directory
        /// </param>
        [PublicAPI]
        public static void AddLuceneSearch(
            [NotNull] this MarkDocsExtensionConfigurationBuilder configuration,
            [NotNull] string indexDirectory
            )
        {
            configuration.AddSearch(services =>
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var engine = new LuceneSearchEngine(loggerFactory, indexDirectory);
                return engine;
            });
        }
    }
}
