using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Markdig.Helpers;

namespace ITGlobal.MarkDocs.Format.CodeBlockRenderers
{
    /// <summary>
    ///    Based on https://bitbucket.org/xact/cs.ff.xact.lib/src/HEAD/XAct.Diagrams/XAct.Diagrams.Uml.PlantUml/Uml/Services/Implementations/PlantUmlDiagramService.cs
    /// </summary>
    internal static class PlantUml
    {
        public static byte[] Render(string baseUrl,string markup, int? lineNumber)
            => RenderAsync(baseUrl, markup, lineNumber).Result;

        private static async Task<byte[]> RenderAsync(string baseUrl, string markup, int? lineNumber)
        {
            try
            {
                var url = $"{baseUrl}/png/{SerializeUml(markup)}";
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(30);
                    var bytes = await httpClient.GetByteArrayAsync(url);
                    return bytes;
                }
            }
            catch (HttpRequestException e)
            {
                MarkdownRenderingContext.RenderContext?.Error($"Failed to render PlantUML markup. {e.Message}", lineNumber, e);
                return Array.Empty<byte>();
            }
        }

        private static string SerializeUml(string markup)
        {
            var compressed = Compress(markup);
            return Encode64(compressed);
        }
        
        private static string Encode64(byte[] bytes)
        {
            var buffer = StringBuilderCache.Local();

            for (var i = 0; i < bytes.Length; i += 3)
            {
                if (i + 2 == bytes.Length)
                {
                    buffer.Append(Append3Bytes(bytes[i], bytes[i + 1], 0));
                }
                else if (i + 1 == bytes.Length)
                {
                    buffer.Append(Append3Bytes(bytes[i], 0, 0));
                }
                else
                {
                    buffer.Append(Append3Bytes(bytes[i], bytes[i + 1], bytes[i + 2]));
                }
            }

            return buffer.ToString();
        }

        private static string Append3Bytes(int b1, int b2, int b3)
        {
            const int sixtyThree = 0x3F;

            var c1 = b1 >> 2;
            var c2 = ((b1 & 0x3) << 4) | (b2 >> 4);
            var c3 = ((b2 & 0xF) << 2) | (b3 >> 6);
            var c4 = b3 & sixtyThree;

            return string.Concat(Encode6Bit(c1 & sixtyThree), Encode6Bit(c2 & sixtyThree), Encode6Bit(c3 & sixtyThree), Encode6Bit(c4 & sixtyThree));
        }

        private static string Encode6Bit(int b)
        {
            if (b < 10)
            {
                return ConvertFromUtf32(48 + b);
            }

            b -= 10;

            if (b < 26)
            {
                return ConvertFromUtf32(65 + b);
            }

            b -= 26;

            if (b < 26)
            {
                return ConvertFromUtf32(97 + b);
            }

            b -= 26;

            if (b == 0)
            {
                return "-";
            }

            if (b == 1)
            {
                return "_";
            }

            return "?";
        }
        
        private static string ConvertFromUtf32(int utf32)
        {
            if (utf32 < 0 || utf32 > 1114111 || utf32 >= 55296 && utf32 <= 57343)
            {
                throw new ArgumentOutOfRangeException(nameof(utf32));
            }

            if (utf32 < 65536)
            {
                return char.ToString((char)utf32);
            }

            utf32 -= 65536;
            return new string(new []
            {
                (char) (utf32 / 1024 + 55296),
                (char) (utf32 % 1024 + 56320)
            });
        }
        
        private static byte[] Compress(string text)
        {
            using (var output = new MemoryStream())
            {
                using (var gzip = new DeflateStream(output, System.IO.Compression.CompressionMode.Compress))
                using (var writer = new StreamWriter(gzip, System.Text.Encoding.UTF8))
                {
                    writer.Write(text);
                }

                return output.ToArray();
            }
        }
    }
}