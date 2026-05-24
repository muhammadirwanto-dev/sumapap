using Xunit;
using Sumapap.Queries.Utils;

namespace Sumapap.Queries.Tests.Utils
{
    public class CursorEncryptionTests
    {
        [Theory]
        [InlineData(123)]
        [InlineData(0)]
        [InlineData(-456)]
        [InlineData(int.MaxValue)]
        [InlineData(int.MinValue)]
        public void EncodeCursor_WithInteger_ShouldEncodeAndDecode(int value)
        {
            // Act
            var encoded = CursorEncryption.EncodeCursor(value);
            var decoded = CursorEncryption.DecodeCursor(encoded, typeof(int));

            // Assert
            Assert.NotNull(encoded);
            Assert.NotEmpty(encoded);
            Assert.Equal(value, decoded);
        }

        [Theory]
        [InlineData("hello")]
        [InlineData("")]
        [InlineData("special!@#$%^&*()")]
        [InlineData("unicode: 你好世界")]
        public void EncodeCursor_WithString_ShouldEncodeAndDecode(string value)
        {
            // Act
            var encoded = CursorEncryption.EncodeCursor(value);
            var decoded = CursorEncryption.DecodeCursor(encoded, typeof(string));

            // Assert
            Assert.NotNull(encoded);
            Assert.Equal(value, decoded);
        }

        [Theory]
        [InlineData(123.456)]
        [InlineData(0.0)]
        [InlineData(-789.012)]
        [InlineData(double.MaxValue)]
        [InlineData(double.MinValue)]
        public void EncodeCursor_WithDouble_ShouldEncodeAndDecode(double value)
        {
            // Act
            var encoded = CursorEncryption.EncodeCursor(value);
            var decoded = CursorEncryption.DecodeCursor(encoded, typeof(double));

            // Assert
            Assert.NotNull(encoded);
            Assert.Equal(value, decoded);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void EncodeCursor_WithBoolean_ShouldEncodeAndDecode(bool value)
        {
            // Act
            var encoded = CursorEncryption.EncodeCursor(value);
            var decoded = CursorEncryption.DecodeCursor(encoded, typeof(bool));

            // Assert
            Assert.NotNull(encoded);
            Assert.Equal(value, decoded);
        }

        [Fact]
        public void EncodeCursor_WithLong_ShouldEncodeAndDecode()
        {
            // Arrange
            long value = 9876543210L;

            // Act
            var encoded = CursorEncryption.EncodeCursor(value);
            var decoded = CursorEncryption.DecodeCursor(encoded, typeof(long));

            // Assert
            Assert.NotNull(encoded);
            Assert.Equal(value, decoded);
        }

        [Fact]
        public void EncodeCursor_ResultShouldBeBase64()
        {
            // Arrange
            var value = "test";

            // Act
            var encoded = CursorEncryption.EncodeCursor(value);

            // Assert
            Assert.NotNull(encoded);
            Assert.Matches("^[A-Za-z0-9+/]*={0,2}$", encoded); // Base64 regex pattern
        }

        [Fact]
        public void DecodeCursor_WithInvalidBase64_ShouldThrowException()
        {
            // Arrange
            var invalidBase64 = "not-valid-base64!!!";

            // Act & Assert
            Assert.Throws<FormatException>(() =>
                CursorEncryption.DecodeCursor(invalidBase64, typeof(string)));
        }

        [Fact]
        public void DecodeCursor_WithInvalidTypeConversion_ShouldThrowException()
        {
            // Arrange
            var encoded = CursorEncryption.EncodeCursor("not-a-number");

            // Act & Assert
            Assert.ThrowsAny<Exception>(() =>
                CursorEncryption.DecodeCursor(encoded, typeof(int)));
        }

        [Theory]
        [InlineData(2024, 12, 25, 10, 30, 45)]
        [InlineData(2000, 1, 1, 0, 0, 0)]
        public void EncodeCursor_WithDateTime_ShouldEncodeAndDecode(int year, int month, int day, int hour, int minute, int second)
        {
            // Arrange
            var value = new DateTime(year, month, day, hour, minute, second);

            // Act
            var encoded = CursorEncryption.EncodeCursor(value);
            var decoded = CursorEncryption.DecodeCursor(encoded, typeof(DateTime));

            // Assert
            Assert.NotNull(encoded);
            Assert.Equal(value, decoded);
        }

        [Fact]
        public void EncodeCursor_MultipleCalls_WithSameValue_ShouldProduceSameResult()
        {
            // Arrange
            var value = 12345;

            // Act
            var encoded1 = CursorEncryption.EncodeCursor(value);
            var encoded2 = CursorEncryption.EncodeCursor(value);

            // Assert
            Assert.Equal(encoded1, encoded2);
        }
    }
}
