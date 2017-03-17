using System;
using System.IO;
using System.Security.Cryptography;

namespace ITGlobal.MarkDocs.Storage
{
    internal static class FileHasher
    {
        public static string ComputeFileHash(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                using (var sha = SHA256.Create())
                {
                    var checksum = sha.ComputeHash(stream);
                    return BitConverter.ToString(checksum).Replace("-", "");
                }
            }
        }
    }
}