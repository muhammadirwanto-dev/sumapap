using Sumapap.Queries.Abstractions.Sorting;
using Xunit;

namespace Sumapap.Queries.Tests.Sorting
{
    public class SortConfigurationTests
    {
        [Fact]
        public void By_ShouldAddSortDescriptor()
        {
            // Arrange & Act
            var config = new SortConfiguration().By("Name");

            // Assert
            Assert.Single(config.Sorts);
            Assert.Equal("Name", config.Sorts[0].Field);
            Assert.Equal(SortDirection.Asc, config.Sorts[0].Direction);
        }

        [Fact]
        public void By_WithDirection_ShouldSetDirection()
        {
            // Arrange & Act
            var config = new SortConfiguration().By("Age", SortDirection.Desc);

            // Assert
            Assert.Single(config.Sorts);
            Assert.Equal("Age", config.Sorts[0].Field);
            Assert.Equal(SortDirection.Desc, config.Sorts[0].Direction);
        }

        [Fact]
        public void ThenBy_ShouldAddSecondarySort()
        {
            // Arrange & Act
            var config = new SortConfiguration()
                .By("LastName")
                .ThenBy("FirstName");

            // Assert
            Assert.Equal(2, config.Sorts.Count);
            Assert.Equal("LastName", config.Sorts[0].Field);
            Assert.Equal("FirstName", config.Sorts[1].Field);
        }

        [Fact]
        public void ThenBy_WithDirection_ShouldSetDirection()
        {
            // Arrange & Act
            var config = new SortConfiguration()
                .By("Category", SortDirection.Asc)
                .ThenBy("Price", SortDirection.Desc);

            // Assert
            Assert.Equal(2, config.Sorts.Count);
            Assert.Equal(SortDirection.Asc, config.Sorts[0].Direction);
            Assert.Equal(SortDirection.Desc, config.Sorts[1].Direction);
        }

        [Fact]
        public void MultipleThenBy_ShouldMaintainOrder()
        {
            // Arrange & Act
            var config = new SortConfiguration()
                .By("A")
                .ThenBy("B")
                .ThenBy("C")
                .ThenBy("D");

            // Assert
            Assert.Equal(4, config.Sorts.Count);
            Assert.Equal("A", config.Sorts[0].Field);
            Assert.Equal("B", config.Sorts[1].Field);
            Assert.Equal("C", config.Sorts[2].Field);
            Assert.Equal("D", config.Sorts[3].Field);
        }

        [Fact]
        public void By_AfterThenBy_ShouldStartNewSortChain()
        {
            // Arrange & Act
            var config = new SortConfiguration()
                .By("Field1")
                .ThenBy("Field2")
                .By("Field3"); // New primary sort

            // Assert
            // Last By() should replace all previous sorts
            Assert.Single(config.Sorts);
            Assert.Equal("Field3", config.Sorts[0].Field);
        }

        [Fact]
        public void DefaultConstructor_ShouldInitializeEmptySortsList()
        {
            // Arrange & Act
            var config = new SortConfiguration();

            // Assert
            Assert.NotNull(config.Sorts);
            Assert.Empty(config.Sorts);
        }
    }

    public class SortDescriptorTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange & Act
            var descriptor = new SortDescriptor("FieldName", SortDirection.Desc);

            // Assert
            Assert.Equal("FieldName", descriptor.Field);
            Assert.Equal(SortDirection.Desc, descriptor.Direction);
        }

        [Fact]
        public void Constructor_WithDefaultDirection_ShouldBeAscending()
        {
            // Arrange & Act
            var descriptor = new SortDescriptor("FieldName");

            // Assert
            Assert.Equal("FieldName", descriptor.Field);
            Assert.Equal(SortDirection.Asc, descriptor.Direction);
        }

        [Theory]
        [InlineData(SortDirection.Asc)]
        [InlineData(SortDirection.Desc)]
        public void Constructor_WithDifferentDirections_ShouldSetDirection(SortDirection direction)
        {
            // Arrange & Act
            var descriptor = new SortDescriptor("Field", direction);

            // Assert
            Assert.Equal(direction, descriptor.Direction);
        }

        [Fact]
        public void Constructor_WithNullField_ShouldNotThrow()
        {
            // Arrange & Act
            var descriptor = new SortDescriptor(null!);

            // Assert
            Assert.Null(descriptor.Field);
        }
    }
}
