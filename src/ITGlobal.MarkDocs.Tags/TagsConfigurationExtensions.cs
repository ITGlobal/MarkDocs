using ITGlobal.MarkDocs.Extensions;
using ITGlobal.MarkDocs.Tags.Impl;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Tags
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsExtensionOptions"/> to configure tags extension
    /// </summary>
    [PublicAPI]
    public static class TagsConfigurationExtensions
    {
        /// <summary>
        ///     Adds tags support into MarkDocs
        /// </summary>
        /// <param name="configuration">
        ///     Configuration builder
        /// </param>
        [PublicAPI]
        public static void AddTags([NotNull] this MarkDocsExtensionOptions configuration)
        {
            configuration.Use<TagsExtensionFactory>();
        }
    }
}