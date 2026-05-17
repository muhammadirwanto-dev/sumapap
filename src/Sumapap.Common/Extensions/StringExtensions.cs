using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace Sumapap.Common.Extensions
{
    public static class StringExtensions
    {
        extension(string? origin)
        {
            [return: NotNullIfNotNull(nameof(origin))]
            public string? ToKebabCase()
            {
                if (string.IsNullOrEmpty(origin))
                {
                    return origin;
                }

                return ComputedRegex.OutsideBracesRegex()
                    .Replace(origin, match => ComputedRegex.CaseBoundaryRegex()
                        .Replace(match.Value, "$1-$2")
                        .ToLower());
            }

            [return: NotNullIfNotNull(nameof(origin))]
            public string? ToSnakeCase()
            {
                if (string.IsNullOrEmpty(origin))
                {
                    return origin;
                }

                return ComputedRegex.OutsideBracesRegex()
                    .Replace(origin, match => ComputedRegex.CaseBoundaryRegex()
                        .Replace(match.Value, "$1_$2")
                        .ToLower());
            }

            [return: NotNullIfNotNull(nameof(origin))]
            public string? ToUpperSnakeCase()
            {
                if (string.IsNullOrEmpty(origin))
                {
                    return origin;
                }

                return ComputedRegex.OutsideBracesRegex()
                    .Replace(origin, match => ComputedRegex.CaseBoundaryRegex()
                        .Replace(match.Value, "$1_$2")
                        .ToUpper());
            }

            [return: NotNullIfNotNull(nameof(origin))]
            public string? Sanitize()
            {
                if (string.IsNullOrEmpty(origin))
                {
                    return origin;
                }

                // Replace newline and carriage return characters
                origin = origin.Replace("\n", "\\n").Replace("\r", "\\r");

                // Escape additional unsafe characters (e.g., for HTML logs)
                origin = System.Web.HttpUtility.HtmlEncode(origin);

                return origin;
            }

            [return: NotNullIfNotNull(nameof(origin))]
            public SecureString? ToSecureString()
            {
                if (string.IsNullOrWhiteSpace(origin))
                {
                    return null;
                }

                var secured = new SecureString();

                foreach (char c in origin.ToCharArray())
                {
                    secured.AppendChar(c);
                }

                return secured;
            }
        }
    }
}
