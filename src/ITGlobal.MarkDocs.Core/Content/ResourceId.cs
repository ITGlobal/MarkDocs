using System.IO;

namespace ITGlobal.MarkDocs.Content
{
    internal static class ResourceId
    {
        /// <summary>
        ///     Normalizes resource ID
        /// </summary>
        public static void Normalize(ref string id)
        {
            if (string.IsNullOrEmpty(id) || id == "/")
            {
                id = "/";
                return;
            }

            id = Path.Combine(Path.GetDirectoryName(id), Path.GetFileName(id));
            id = id.Replace(Path.DirectorySeparatorChar, '/');
            id = id.Replace(Path.AltDirectorySeparatorChar, '/');
            id = id.ToLowerInvariant();

            if (!id.StartsWith("/"))
            {
                id = "/" + id;
            }
        }

    }
}