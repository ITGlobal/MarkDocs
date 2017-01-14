using System;
using ITGlobal.MarkDocs.Comments.Data;
using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsExtensionConfigurationBuilder"/> to configure page comments
    /// </summary>
    [PublicAPI]
    public static class MarkdocsConfigurationExtensions
    {
        /// <summary>
        ///     Adds comment support into MarkDocs
        /// </summary>
        /// <param name="configuration">
        ///     Configuration builder
        /// </param>
        /// <param name="repositoryFactory">
        ///     Comment data repository
        /// </param>
        [PublicAPI]
        public static void AddComments(
            [NotNull] this MarkDocsExtensionConfigurationBuilder configuration,
            [NotNull] Func<IServiceProvider, ICommentDataRepository> repositoryFactory)
        {
            configuration.Add(sp =>
            {
                var repository = repositoryFactory(sp);
                var format = sp.GetRequiredService<IFormat>();
                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
                var factory = new CommentsExtensionFactory(repository, format, loggerFactory);
                return factory;
            });
        }

        /// <summary>
        ///     Adds comment support into MarkDocs
        /// </summary>
        /// <param name="configuration">
        ///     Configuration builder
        /// </param>
        /// <param name="repositoryFactory">
        ///     Comment data repository
        /// </param>
        [PublicAPI]
        public static void AddComments(
            [NotNull] this MarkDocsExtensionConfigurationBuilder configuration,
            [NotNull] ICommentDataRepository repositoryFactory)
        {
            AddComments(configuration, _ => repositoryFactory);
        }
    }
}

