using Xunit;
using Sumapap.Queries.Abstractions.Filtering;

namespace Sumapap.Queries.Tests.Filtering
{
    public class FilterConfigurationTests
    {
        [Fact]
        public void WithFilters_ShouldSetFilters()
        {
            // Arrange
            var filters = new List<FilterDescriptor>
            {
                new("Name", FilterOperator.Equals, "Test")
            };

            // Act
            var config = new FilterConfiguration().WithFilters(filters);

            // Assert
            Assert.Single(config.Filters);
            Assert.Equal("Name", config.Filters.First().Field);
        }

        [Fact]
        public void WithOperator_ShouldSetCompositeOperator()
        {
            // Arrange & Act
            var config = new FilterConfiguration().WithOperator(CompositeOperator.Or);

            // Assert
            Assert.Equal(CompositeOperator.Or, config.Operator);
        }

        [Fact]
        public void WithOperator_DefaultShouldBeAnd()
        {
            // Arrange & Act
            var config = new FilterConfiguration();

            // Assert
            Assert.Equal(CompositeOperator.And, config.Operator);
        }

        [Fact]
        public void HavingSubGroups_ShouldSetSubGroups()
        {
            // Arrange
            var subGroup = new FilterConfiguration();
            subGroup.WithFilters([new FilterDescriptor("Age", FilterOperator.GreaterThan, 18)]);

            // Act
            var config = new FilterConfiguration();
            config.HavingSubGroups([subGroup]);

            // Assert
            Assert.Single(config.SubGroups);
            Assert.Single(config.SubGroups.First().Filters);
        }

        [Fact]
        public void FluentChaining_ShouldWorkCorrectly()
        {
            // Arrange & Act
            var subGroup = new FilterConfiguration();
            subGroup.WithFilters([new FilterDescriptor("Age", FilterOperator.LessThan, 30)]);

            var config = new FilterConfiguration();
            config.WithFilters([
                new FilterDescriptor("Name", FilterOperator.Contains, "John")
            ]);
            config.WithOperator(CompositeOperator.Or);
            config.HavingSubGroups([subGroup]);

            // Assert
            Assert.Single(config.Filters);
            Assert.Equal(CompositeOperator.Or, config.Operator);
            Assert.Single(config.SubGroups);
        }

        [Fact]
        public void WithFilters_EmptyCollection_ShouldBeAllowed()
        {
            // Arrange & Act
            var config = new FilterConfiguration().WithFilters([]);

            // Assert
            Assert.Empty(config.Filters);
        }

        [Fact]
        public void HavingSubGroups_EmptyCollection_ShouldBeAllowed()
        {
            // Arrange & Act
            var config = new FilterConfiguration().HavingSubGroups([]);

            // Assert
            Assert.Empty(config.SubGroups);
        }

        [Fact]
        public void WithFilters_WithNullField_ShouldNotThrow()
        {
            // Arrange
            var filters = new List<FilterDescriptor>
            {
                new(null!, FilterOperator.Equals, "Value")
            };

            // Act
            var config = new FilterConfiguration().WithFilters(filters);

            // Assert
            Assert.Single(config.Filters);
            Assert.Null(config.Filters.First().Field);
        }
    }

    public class FilterDescriptorTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange & Act
            var descriptor = new FilterDescriptor("Name", FilterOperator.Equals, "Test");

            // Assert
            Assert.Equal("Name", descriptor.Field);
            Assert.Equal(FilterOperator.Equals, descriptor.Operator);
            Assert.Equal("Test", descriptor.Value);
        }

        [Theory]
        [InlineData(FilterOperator.Equals)]
        [InlineData(FilterOperator.NotEquals)]
        [InlineData(FilterOperator.GreaterThan)]
        [InlineData(FilterOperator.LessThan)]
        [InlineData(FilterOperator.GreaterThanOrEqual)]
        [InlineData(FilterOperator.LessThanOrEqual)]
        [InlineData(FilterOperator.Contains)]
        [InlineData(FilterOperator.StartsWith)]
        [InlineData(FilterOperator.EndsWith)]
        public void Constructor_WithDifferentOperators_ShouldSetOperator(FilterOperator op)
        {
            // Arrange & Act
            var descriptor = new FilterDescriptor("Field", op, "Value");

            // Assert
            Assert.Equal(op, descriptor.Operator);
        }

        [Fact]
        public void Constructor_WithNullValue_ShouldBeAllowed()
        {
            // Arrange & Act
            var descriptor = new FilterDescriptor("Field", FilterOperator.Equals, null);

            // Assert
            Assert.Null(descriptor.Value);
        }

        [Fact]
        public void Constructor_WithComplexValue_ShouldSetValue()
        {
            // Arrange
            var complexValue = new { Name = "Test", Age = 25 };

            // Act
            var descriptor = new FilterDescriptor("Field", FilterOperator.Equals, complexValue);

            // Assert
            Assert.Equal(complexValue, descriptor.Value);
        }
    }
}
