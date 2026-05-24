using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.DependencyInjection.Tests
{
    public class SumapapServiceCollectionExtensionsTests
    {
        [Fact]
        public void AddSumapap_WhenCalled_ReturnsBuilder()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            var builder = services.AddSumapap();

            // Assert
            Assert.NotNull(builder);
            Assert.IsAssignableFrom<ISumapapServiceBuilder>(builder);
        }

        [Fact]
        public void AddSumapap_WhenServicesIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IServiceCollection services = null!;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => services.AddSumapap());
        }

        [Fact]
        public void AddSumapap_Build_ReturnsOriginalServiceCollection()
        {
            // Arrange
            var services = new ServiceCollection();
            var builder = services.AddSumapap();

            // Act
            var result = builder.Build();

            // Assert
            Assert.Same(services, result);
        }
    }
}
