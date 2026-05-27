using Sumapap.Queries.Abstractions.Paging;
using Xunit;

namespace Sumapap.Queries.Tests.Paging
{
    public class OffsetPagingTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange & Act
            var paging = new OffsettPaginationConfiguration(3, 25);

            // Assert
            Assert.Equal(3, paging.Page);
            Assert.Equal(25, paging.PageSize);
            Assert.Equal(50, paging.Offset);
        }

        [Theory]
        [InlineData(1, 10, 0)]
        [InlineData(2, 20, 20)]
        [InlineData(3, 50, 100)]
        [InlineData(10, 100, 900)]
        public void Constructor_WithDifferentValues_ShouldCalculateOffsetCorrectly(int page, int pageSize, int expectedOffset)
        {
            // Arrange & Act
            var paging = new OffsettPaginationConfiguration(page, pageSize);

            // Assert
            Assert.Equal(page, paging.Page);
            Assert.Equal(pageSize, paging.PageSize);
            Assert.Equal(expectedOffset, paging.Offset);
        }

        [Fact]
        public void Constructor_WithDefaultValues_ShouldUseDefaults()
        {
            // Arrange & Act
            var paging = new OffsettPaginationConfiguration();

            // Assert
            Assert.Equal(1, paging.Page);
            Assert.Equal(20, paging.PageSize);
            Assert.Equal(0, paging.Offset);
        }
    }

    public class CursorPagingTests
    {
        [Fact]
        public void Constructor_WithAllParameters_ShouldInitializeProperties()
        {
            // Arrange & Act
            var paging = new CursorPaginationConfiguration("Id", "cursorValue", 20, CursorDirection.Backward);

            // Assert
            Assert.Equal("Id", paging.CursorField);
            Assert.Equal("cursorValue", paging.Cursor);
            Assert.Equal(20, paging.Limit);
            Assert.Equal(CursorDirection.Backward, paging.Direction);
        }

        [Fact]
        public void Constructor_WithMinimalParameters_ShouldUseDefaults()
        {
            // Arrange & Act
            var paging = new CursorPaginationConfiguration("Id");

            // Assert
            Assert.Equal("Id", paging.CursorField);
            Assert.Null(paging.Cursor);
            Assert.Equal(20, paging.Limit); // Default limit
            Assert.Equal(CursorDirection.Forward, paging.Direction); // Default direction
        }

        [Fact]
        public void Constructor_WithCursorFieldAndCursor_ShouldSetBoth()
        {
            // Arrange & Act
            var paging = new CursorPaginationConfiguration("CreatedAt", "2024-01-01");

            // Assert
            Assert.Equal("CreatedAt", paging.CursorField);
            Assert.Equal("2024-01-01", paging.Cursor);
        }

        [Theory]
        [InlineData(CursorDirection.Forward)]
        [InlineData(CursorDirection.Backward)]
        public void Constructor_WithDifferentDirections_ShouldSetDirection(CursorDirection direction)
        {
            // Arrange & Act
            var paging = new CursorPaginationConfiguration("Id", "cursor", 15, direction);

            // Assert
            Assert.Equal(direction, paging.Direction);
        }

        [Fact]
        public void Constructor_WithCustomLimit_ShouldSetLimit()
        {
            // Arrange & Act
            var paging = new CursorPaginationConfiguration("Id", "cursor", 50);

            // Assert
            Assert.Equal(50, paging.Limit);
        }

        [Fact]
        public void Constructor_WithNullCursor_ShouldBeAllowed()
        {
            // Arrange & Act
            var paging = new CursorPaginationConfiguration("Id", null);

            // Assert
            Assert.Equal("Id", paging.CursorField);
            Assert.Null(paging.Cursor);
        }
    }

    public class PageInfoTests
    {
        [Fact]
        public void Constructor_ShouldInitializeProperties()
        {
            // Arrange & Act
            var pageInfo = new PageInfo(true, false, "startCursor", "endCursor");

            // Assert
            Assert.True(pageInfo.HasNextPage);
            Assert.False(pageInfo.HasPreviousPage);
            Assert.Equal("startCursor", pageInfo.StartCursor);
            Assert.Equal("endCursor", pageInfo.EndCursor);
        }

        [Theory]
        [InlineData(true, true)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        [InlineData(false, false)]
        public void Constructor_WithDifferentPageFlags_ShouldSetCorrectly(bool hasNext, bool hasPrevious)
        {
            // Arrange & Act
            var pageInfo = new PageInfo(hasNext, hasPrevious, null, null);

            // Assert
            Assert.Equal(hasNext, pageInfo.HasNextPage);
            Assert.Equal(hasPrevious, pageInfo.HasPreviousPage);
        }

        [Fact]
        public void Constructor_WithNullCursors_ShouldBeAllowed()
        {
            // Arrange & Act
            var pageInfo = new PageInfo(false, false, null, null);

            // Assert
            Assert.Null(pageInfo.StartCursor);
            Assert.Null(pageInfo.EndCursor);
        }

        [Fact]
        public void Constructor_WithEmptyCursors_ShouldSetEmptyStrings()
        {
            // Arrange & Act
            var pageInfo = new PageInfo(true, true, "", "");

            // Assert
            Assert.Equal("", pageInfo.StartCursor);
            Assert.Equal("", pageInfo.EndCursor);
        }
    }
}
