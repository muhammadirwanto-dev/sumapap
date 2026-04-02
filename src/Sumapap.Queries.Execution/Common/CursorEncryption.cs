using System.Globalization;
using System.Text;

namespace Sumapap.Queries.Execution.Common
{
    public static class CursorEncryption
    {
        public static string EncodeCursor(object value)
        {
            var enc = Encoding.UTF8.GetBytes(
                Convert.ToString(value, CultureInfo.InvariantCulture)!);

            return Convert.ToBase64String(enc);
        }

        public static object DecodeCursor(string cursor, Type targetType)
        {
            var raw = Encoding.UTF8.GetString(
                Convert.FromBase64String(cursor));

            return Convert.ChangeType(raw, targetType, CultureInfo.InvariantCulture)!;
        }
    }
}
