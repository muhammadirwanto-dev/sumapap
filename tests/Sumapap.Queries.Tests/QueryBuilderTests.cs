using Xunit;
using Sumapap.Queries;
using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Abstractions.Filtering;
using Sumapap.Queries.Abstractions.Sorting;
using Sumapap.Queries.Abstractions.Paging;

namespace Sumapap.Queries.Tests
{
    public class QueryBuilderTests
    {
        [Fact]
        public void Build_WithNoConfiguration_ShouldCreateEmptyQuery()
        {
            // Arrange
            var builder = new QueryBuilder();

            // Act
            var query = builder.Build();

            // Assert
            Assert.NotNull(query);
            Assert.NotNull(query.Filters);
            Assert.NotNull(query.Sort);
            Assert.Null(query.OffsetPaging);
            Assert.Null(query.CursorPaging);
            Assert.False(query.UsesOffsetPaging);
            Assert.False(query.UsesCursorPaging);
        }

        [Fact]
        public void Build_WithFilters_ShouldIncludeFilters()
        {
            // Arrange
            var filters = new FilterConfiguration();
            filters.WithFilters([
                new FilterDescriptor("Name", FilterOperator.Equals, "Test")
            ]);
            var builder = new QueryBuilder();

            // Act
            var query = builder.WithFilters(filters).Build();

            // Assert
            Assert.NotNull(query.Filters);
            Assert.Single(query.Filters.Filters);
            Assert.Equal("Name", query.Filters.Filters.First().Field);
        }

        [Fact]
        public void Build_WithSort_ShouldIncludeSort()
        {
            // Arrange
            var sort = new SortConfiguration()
                .By("Name", SortDirection.Asc)
                .ThenBy("Age", SortDirection.Desc);
            var builder = new QueryBuilder();

            // Act
            var query = builder.WithSort(sort).Build();

            // Assert
            Assert.NotNull(query.Sort);
            Assert.Equal(2, query.Sort.Sorts.Count);
            Assert.Equal("Name", query.Sort.Sorts[0].Field);
            Assert.Equal(SortDirection.Asc, query.Sort.Sorts[0].Direction);
        }

        [Fact]
        public void Build_WithOffsetPaging_ShouldSetOffsetPaging()
        {
            // Arrange
            var builder = new QueryBuilder();

            // Act
            var query = builder.UseOffsetPaging(new OffsettPaginationConfiguration(2, 20)).Build();

            // Assert
            Assert.NotNull(query.OffsetPaging);
            Assert.Equal(2, query.OffsetPaging.Page);
            Assert.Equal(20, query.OffsetPaging.PageSize);
            Assert.True(query.UsesOffsetPaging);
            Assert.False(query.UsesCursorPaging);
            Assert.Null(query.CursorPaging);
        }

        [Fact]
        public void Build_WithCursorPaging_ShouldSetCursorPaging()
        {
            // Arrange
            var builder = new QueryBuilder();

            // Act
            var query = builder.UseCursorPaging("Id", "cursor123", 25, CursorDirection.Forward).Build();

            // Assert
            Assert.NotNull(query.CursorPaging);
            Assert.Equal("Id", query.CursorPaging.CursorField);
            Assert.Equal("cursor123", query.CursorPaging.Cursor);
            Assert.Equal(25, query.CursorPaging.Limit);
            Assert.Equal(CursorDirection.Forward, query.CursorPaging.Direction);
            Assert.True(query.UsesCursorPaging);
            Assert.False(query.UsesOffsetPaging);
            Assert.Null(query.OffsetPaging);
        }

        [Fact]
        public void UseOffsetPaging_AfterCursorPaging_ShouldReplaceCursorWithOffset()
        {
            // Arrange
            var builder = new QueryBuilder();

            // Act
            var query = builder
                .UseCursorPaging("Id", "cursor", 10)
                .UseOffsetPaging(new OffsettPaginationConfiguration(1, 20))
                .Build();

            // Assert
            Assert.NotNull(query.OffsetPaging);
            Assert.Null(query.CursorPaging);
            Assert.True(query.UsesOffsetPaging);
            Assert.False(query.UsesCursorPaging);
        }

        [Fact]
        public void UseCursorPaging_AfterOffsetPaging_ShouldReplaceOffsetWithCursor()
        {
            // Arrange
            var builder = new QueryBuilder();

            // Act
            var query = builder
                .UseOffsetPaging(new OffsettPaginationConfiguration(1, 20))
                .UseCursorPaging("Id")
                .Build();

            // Assert
            Assert.NotNull(query.CursorPaging);
            Assert.Null(query.OffsetPaging);
            Assert.True(query.UsesCursorPaging);
            Assert.False(query.UsesOffsetPaging);
        }

        [Fact]
        public void WithOptionalFilter_WhenNull_ShouldNotSetFilter()
        {
            // Arrange
            var builder = new QueryBuilder();

            // Act
            var query = builder.WithOptionalFilter(null).Build();

            // Assert
            Assert.NotNull(query.Filters);
            Assert.Empty(query.Filters.Filters);
        }

        [Fact]
        public void WithOptionalSort_WhenNull_ShouldNotSetSort()
        {
            // Arrange
            var builder = new QueryBuilder();

            // Act
            var query = builder.WithOptionalSort(null).Build();

            // Assert
            Assert.NotNull(query.Sort);
            Assert.Empty(query.Sort.Sorts);
        }

        [Fact]
        public void Build_FluentChaining_ShouldWorkCorrectly()
        {
            // Arrange
            var filters = new FilterConfiguration();
            filters.WithFilters([new FilterDescriptor("Status", FilterOperator.Equals, "Active")]);
            var sort = new SortConfiguration();
            sort.By("Name");

            // Act
            var query = new QueryBuilder()
                .WithFilters(filters)
                .WithSort(sort)
                .UseOffsetPaging(new OffsettPaginationConfiguration(1, 10))
                .Build();

            // Assert
            Assert.NotNull(query.Filters);
            Assert.NotNull(query.Sort);
            Assert.NotNull(query.OffsetPaging);
            Assert.Single(query.Filters.Filters);
            Assert.Single(query.Sort.Sorts);
        }
    }
}
