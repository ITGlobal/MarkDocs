using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ITGlobal.MarkDocs.Storage
{
    internal static class HashUtils
    {
        public static string HashDirectory(string path)
        {
            var files = Directory.GetFiles(
                    path, "*",
                    SearchOption.AllDirectories)
                .OrderBy(p => p)
                .ToList();

#if !NET45
            using (var algorithm = IncrementalHash.CreateHash(HashAlgorithmName.MD5))
            {
                foreach (var file in files)
                {
                    var relativePath = file.Substring(path.Length + 1);
                    var pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());

                    algorithm.AppendData(pathBytes);

                    // hash contents
                    var contentBytes = File.ReadAllBytes(file);
                    algorithm.AppendData(contentBytes);
                }

                var hash = algorithm.GetHashAndReset();
                var hashStr = BitConverter.ToString(hash).Replace("-", "").ToLower();
                return hashStr;
            }
#else
            using (var algorithm = MD5.Create())
            {
                for (var i = 0; i < files.Count; i++)
                {
                    var file = files[i];

                    // hash path
                    var relativePath = file.Substring(path.Length + 1);
                    var pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());

                    algorithm.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);
                    algorithm.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                    // hash contents
                    byte[] contentBytes = File.ReadAllBytes(file);
                    if (i == files.Count - 1)
                    {
                        algorithm.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                    }
                    else
                    {
                        algorithm.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                    }
                }

                return BitConverter.ToString(algorithm.Hash).Replace("-", "").ToLower();
            }
#endif
        }
    }
}