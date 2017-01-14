using ITGlobal.MarkDocs.Extensions;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Tags
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
        [PublicAPI]
        public static void AddTags([NotNull] this MarkDocsExtensionConfigurationBuilder configuration)
        {
            configuration.Add<TagsExtensionFactory>();
        }
    }
}