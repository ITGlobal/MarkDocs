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
            normalizedPath = NormalizePath(normalizedPath);
            return normalizedPath;
        }

        public static string GetRelativePath(string rootDirectory, string path)
        {
            try
            {
                var normalizedRootPath = NormalizePath(rootDirectory);
                var normalizedPath = NormalizePath(path);

                if (!normalizedPath.StartsWith(normalizedRootPath))
                {
                    throw new Exception($"\"{normalizedRootPath}\" is not a valid root for \"{normalizedPath}\"");
                }

                var relativePath = normalizedPath.Substring(normalizedRootPath.Length);
                relativePath = NormalizePath(relativePath);
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

        private static string NormalizePath(string path)
        {
            path= path.Replace(Path.DirectorySeparatorChar, '/');
            path = path.Replace(Path.AltDirectorySeparatorChar, '/');
            while (path.Length > 1 && path[path.Length-1] == '/')
            {
                path = path.Substring(0, path.Length-1);
            }

            return path;
        }
    }
}