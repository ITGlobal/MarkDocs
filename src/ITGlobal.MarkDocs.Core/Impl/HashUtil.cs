using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ITGlobal.MarkDocs.Impl
{
    internal static class HashUtil
    {
        private static HashAlgorithm CreateHashAlgorithm() => SHA1.Create();

        public static string HashStream(Stream stream)
        {
            using (var alg = CreateHashAlgorithm())
            {
                var hashBytes = alg.ComputeHash(stream);
                var hash = GetHashString(hashBytes);
                return hash;
            }
        }

        public static string HashBuffer(byte[] buffer)
        {
            using (var alg = CreateHashAlgorithm())
            {
                var hashBytes = alg.ComputeHash(buffer);
                var hash = GetHashString(hashBytes);
                return hash;
            }
        }

        public static string HashString(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return HashBuffer(bytes);
        }

        public static string HashObject<T>(T obj)
        {
            var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                ContractResolver = new CustomOrderContractResolver()
            });

            return HashString(json);
        }

        private static string GetHashString(byte[] bytes)
        {
            var hash = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
            return hash;
        }

        private sealed class CustomOrderContractResolver : DefaultContractResolver
        {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
            {
                return base.CreateProperties(type, memberSerialization)
                    .OrderBy(_ => _.PropertyName)
                    .ToList();
            }
        }
    }
}