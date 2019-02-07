using ITGlobal.MarkDocs.Tags.Impl;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace ITGlobal.MarkDocs.Tags
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsOptions"/> to configure tags extension
    /// </summary>
    [PublicAPI]
    public static class MarkDocsOptionsExtensions
    {
        /// <summary>
        ///     Adds tags support into MarkDocs
        /// </summary>
        /// <param name="options">
        ///     Configuration builder
        /// </param>
        [NotNull]
        public static MarkDocsOptions AddTags([NotNull] this MarkDocsOptions options)
        {
            options.ConfigureServices(_ => _.AddSingleton<TagsExtensionFactory>());
            return options.AddExtension(_ => _.GetRequiredService<TagsExtensionFactory>());
        }
    }
}