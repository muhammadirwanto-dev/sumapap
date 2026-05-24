using System.Text.RegularExpressions;

namespace Sumapap.Common
{
    /// <summary>
    /// Provides compiled regular expressions for common pattern matching operations.
    /// </summary>
    public partial class ComputedRegex
    {
        /// <summary>
        /// Gets a compiled regular expression that matches alphabetic characters outside of curly braces.
        /// </summary>
        /// <returns>
        /// A <see cref="Regex"/> instance that matches one or more consecutive letters (a-z, A-Z) 
        /// that appear outside of balanced curly brace pairs.
        /// </returns>
        /// <remarks>
        /// This pattern uses a positive lookahead to ensure matched text is not within braces.
        /// Pattern: <c>[a-zA-Z]+(?=(?:[^{}]*{[^{}]*})*[^{}]*$)</c>
        /// </remarks>
        [GeneratedRegex(@"[a-zA-Z]+(?=(?:[^{}]*{[^{}]*})*[^{}]*$)")]
        public static partial Regex OutsideBracesRegex();

        /// <summary>
        /// Gets a compiled regular expression that matches the boundary between lowercase and uppercase letters.
        /// </summary>
        /// <returns>
        /// A <see cref="Regex"/> instance that matches positions where a lowercase letter 
        /// is immediately followed by an uppercase letter.
        /// </returns>
        /// <remarks>
        /// Useful for converting camelCase or PascalCase strings to other formats (e.g., inserting spaces or hyphens).
        /// Pattern: <c>([a-z])([A-Z])</c>
        /// </remarks>
        [GeneratedRegex("([a-z])([A-Z])")]
        public static partial Regex CaseBoundaryRegex();
    }
}
