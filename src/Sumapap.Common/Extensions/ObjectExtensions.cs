using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Sumapap.Common.Extensions
{
    public static class ObjectExtensions
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = false };

        extension(object @object)
        {
            public string GetContentHash()
            {
                if (@object == null)
                {
                    return string.Empty;
                }

                string json = JsonSerializer.Serialize(@object, _jsonOptions);
                byte[] inputBytes = Encoding.UTF8.GetBytes(json);
                byte[] hashBytes = SHA256.HashData(inputBytes);

                return Convert.ToHexString(hashBytes);
            }
        }
    }
}
