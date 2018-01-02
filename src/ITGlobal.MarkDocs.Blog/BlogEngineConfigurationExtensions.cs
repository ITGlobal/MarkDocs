using System;
using ITGlobal.MarkDocs.Format;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Blog
{
    /// <summary>
    ///    Extension methods for setting up MarkDocs Blog Engine services in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class BlogEngineConfigurationExtensions
    {
        /// <summary>
        ///     Adds MarkDocs Blog Engine services into <paramref name="services"/>
        /// </summary>
        /// <param name="services">
        ///     Service collection to configure
        /// </param>
        /// <param name="workingDirectory">
        ///     Path to blog engine working directory
        /// </param>
        /// <param name="urlResolver">
        ///     URL resolver
        /// </param>
        /// <param name="configure">
        ///     Configuration function
        /// </param>
        [PublicAPI]
        public static void AddMarkdocsBlogEngine(
            [NotNull] this IServiceCollection services,
            [NotNull] string workingDirectory,
            [NotNull] IResourceUrlResolver urlResolver,
            [NotNull] Action<BlogEngineConfigurationBuilder> configure)
        {
            var builder = new BlogEngineConfigurationBuilder(workingDirectory, urlResolver);
            configure(builder);

            var factory = builder.Build();
            services.AddSingleton(factory);
        }
    }
}