using System.IO;
using ITGlobal.MarkDocs.Comments.Data;
using ITGlobal.MarkDocs.Extensions;
using JetBrains.Annotations;

namespace ITGlobal.MarkDocs.Comments
{
    /// <summary>
    ///     Extension methods for <see cref="MarkDocsExtensionConfigurationBuilder"/> to configure page comments
    /// </summary>
    [PublicAPI]
    public static class MarkdocsConfigurationExtensions
    {
        /// <summary>
        ///     Adds comment support into MarkDocs with LiteDB-based storage
        /// </summary>
        /// <param name="configuration">
        ///     Configuration builder
        /// </param>
        /// <param name="filename">
        ///     Path to LiteDB database fle
        /// </param>
        [PublicAPI]
        public static void AddLiteDbComments(
            [NotNull] this MarkDocsExtensionConfigurationBuilder configuration,
            [NotNull] string filename)
        {
            var directoryName = Path.GetDirectoryName(filename);
            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
            }

            configuration.AddComments(sp =>
            {
                var storage = new LiteDbCommentDataRepository(filename);
                return storage;
            });
        }
    }
}
