using Sumapap.Common.Extensions;

namespace Sumapap.Common.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("PascalCase", "pascal-case")]
        [InlineData("camelCase", "camel-case")]
        [InlineData("XMLHttpRequest", "xmlhttp-request")]
        [InlineData("IOError", "ioerror")]
        [InlineData("simple", "simple")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void ToKebabCase_ConvertsCorrectly(string? input, string? expected)
        {
            // Act
            var result = input.ToKebabCase();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("PascalCase", "pascal_case")]
        [InlineData("camelCase", "camel_case")]
        [InlineData("XMLHttpRequest", "xmlhttp_request")]
        [InlineData("IOError", "ioerror")]
        [InlineData("simple", "simple")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void ToSnakeCase_ConvertsCorrectly(string? input, string? expected)
        {
            // Act
            var result = input.ToSnakeCase();

            // Assert
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("PascalCase", "PASCAL_CASE")]
        [InlineData("camelCase", "CAMEL_CASE")]
        [InlineData("XMLHttpRequest", "XMLHTTP_REQUEST")]
        [InlineData("IOError", "IOERROR")]
        [InlineData("simple", "SIMPLE")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void ToUpperSnakeCase_ConvertsCorrectly(string? input, string? expected)
        {
            // Act
            var result = input.ToUpperSnakeCase();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Sanitize_RemovesNewlineCharacters()
        {
            // Arrange
            var input = "Line1\nLine2\rLine3";

            // Act
            var result = input.Sanitize();

            // Assert
            Assert.DoesNotContain("\n", result);
            Assert.DoesNotContain("\r", result);
            Assert.Contains("\\n", result);
            Assert.Contains("\\r", result);
        }

        [Fact]
        public void Sanitize_HtmlEncodesUnsafeCharacters()
        {
            // Arrange
            var input = "<script>alert('xss')</script>";

            // Act
            var result = input.Sanitize();

            // Assert
            Assert.DoesNotContain("<script>", result);
            Assert.Contains("&lt;", result);
            Assert.Contains("&gt;", result);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void Sanitize_HandlesEmptyOrNullString(string? input, string? expected)
        {
            // Act
            var result = input.Sanitize();

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToSecureString_ConvertsToSecureString()
        {
            // Arrange
            var input = "MySecretPassword";

            // Act
            var result = input.ToSecureString();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(input.Length, result.Length);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void ToSecureString_ReturnsNullForEmptyOrWhitespace(string? input)
        {
            // Act
            var result = input.ToSecureString();

            // Assert
            Assert.Null(result);
        }
    }
}
