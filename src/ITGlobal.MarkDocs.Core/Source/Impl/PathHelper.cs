using System;
using System.IO;

namespace ITGlobal.MarkDocs.Source.Impl
{
    internal static class PathHelper
    {
        public static string GetAbsolutePath(string rootDirectory, string path)
        {
            var p = path;
            while (p.Length > 0 && (p[0] == '/' || p[0] == '\\'))
            {
                p = p.Substring(1);
            }

            var normalizedPath = p.Length > 0
                ? Path.Combine(rootDirectory, p)
                : rootDirectory;
            normalizedPath = Path.GetFullPath(normalizedPath);
            return normalizedPath;
        }

        public static string GetRelativePath(string rootDirectory, string path)
        {
            try
            {
                var normalizedRootPath = Path.GetFullPath(rootDirectory);
                var normalizedPath = GetAbsolutePath(rootDirectory, path);

                if (!normalizedPath.StartsWith(normalizedRootPath))
                {
                    throw new Exception($"\"{normalizedRootPath}\" is not a valid root for \"{normalizedPath}\"");
                }

                var relativePath = normalizedPath.Substring(normalizedRootPath.Length);
                relativePath = relativePath.Replace(Path.DirectorySeparatorChar, '/');
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, '/');
                while (relativePath.Length > 0 && relativePath[0] == '/')
                {
                    relativePath = relativePath.Substring(1);
                }

                return relativePath;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"GetRelativePath(\"{rootDirectory}\", \"{path}\") failed", e);
            }
        }
    }
}