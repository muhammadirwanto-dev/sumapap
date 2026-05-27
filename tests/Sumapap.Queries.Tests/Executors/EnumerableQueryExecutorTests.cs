using Xunit;
using Sumapap.Queries.Executors;
using Sumapap.Queries.Abstractions;
using Sumapap.Queries.Abstractions.Filtering;
using Sumapap.Queries.Abstractions.Sorting;
using Sumapap.Queries.Abstractions.Paging;

namespace Sumapap.Queries.Tests.Executors
{
    public class EnumerableQueryExecutorTests
    {
        private readonly EnumerableQueryExecutor<TestEntity> _executor;

        public EnumerableQueryExecutorTests()
        {
            _executor = new EnumerableQueryExecutor<TestEntity>();
        }

        [Fact]
        public void Execute_WithNoFiltersOrSorting_ShouldReturnAllItems()
        {
            // Arrange
            var data = CreateTestData();
            var query = new Query();

            // Act
            var result = _executor.Execute(query, data);

            // Assert
            Assert.Equal(5, result.TotalDataCount);
            Assert.Equal(5, result.Items.Count());
        }

        [Fact]
        public void Execute_WithEqualityFilter_ShouldFilterCorrectly()
        {
            // Arrange
            var data = CreateTestData();
            var filters = new FilterConfiguration();
            filters.WithFilters([
                new FilterDescriptor("Name", FilterOperator.Equals, "Alice")
            ]);
            var query = new QueryBuilder().WithFilters(filters).Build();

            // Act
            var result = _executor.Execute(query, data);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal("Alice", result.Items.First().Name);
        }

        [Fact]
        public void Execute_WithGreaterThanFilter_ShouldFilterCorrectly()
        {
            // Arrange
            var data = CreateTestData();
            var filters = new FilterConfiguration();
            filters.WithFilters([
                new FilterDescriptor("Age", FilterOperator.GreaterThan, 25)
            ]);
            var query = new QueryBuilder().WithFilters(filters).Build();

            // Act
            var result = _executor.Execute(query, data);

            // Assert
            Assert.Equal(3, result.Items.Count());
            Assert.All(result.Items, item => Assert.True(item.Age > 25));
        }

        [Fact]
        public void Execute_WithContainsFilter_ShouldFilterCorrectly()
        {
            // Arrange
            var data = CreateTestData();
            var filters = new FilterConfiguration();
            filters.WithFilters([
                new FilterDescriptor("Name", FilterOperator.Contains, "o")
            ]);
            var query = new QueryBuilder().WithFilters(filters).Build();

            // Act
            var result = _executor.Execute(query, data);

            // Assert
            Assert.True(result.Items.Count() >= 1);
            Assert.All(result.Items, item => Assert.Contains("o", item.Name, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void Execute_WithAscendingSort_ShouldSortCorrectly()
        {
            // Arrange
            var data = CreateTestData();
            var sort = new SortConfiguration().By("Age", SortDirection.Asc);
            var query = new QueryBuilder().WithSort(sort).Build();

            // Act
            var result = _executor.Execute(query, data);

            // Assert
            var ages = result.Items.Select(x => x.Age).ToList();
            Assert.Equal(ages.OrderBy(x => x).ToList(), ages);
        }

        [Fact]
        public void Execute_WithDescendingSort_ShouldSortCorrectly()
        {
            // Arrange
            var data = CreateTestData();
            var sort = new SortConfiguration().By("Name", SortDirection.Desc);
            var query = new QueryBuilder().WithSort(sort).Build();

            // Act
            var result = _executor.Execute(query, data);

            // Assert
            var names = result.Items.Select(x => x.Name).ToList();
            Assert.Equal(names.OrderByDescending(x => x).ToList(), names);
        }

        [Fact]
        public void Execute_WithMultipleSorts_ShouldApplySortsInOrder()
        {
            // Arrange
            var data = CreateTestData();
            var sort = new SortConfiguration()
                .By("Age", SortDirection.Desc)
                .ThenBy("Name", SortDirection.Asc);
            var query = new QueryBuilder().WithSort(sort).Build();

            // Act
            var result = _executor.Execute(query, data);

            // Assert
            Assert.NotEmpty(result.Items);
            var items = result.Items.ToList();
            // Verify primary sort (Age descending)
            for (int i = 0; i < items.Count - 1; i++)
            {
                Assert.True(items[i].Age >= items[i + 1].Age);
            }
        }

        [Fact]
        public void Execute_WithOffsetPaging_ShouldReturnCorrectPage()
        {
            // Arrange
            var data = CreateTestData();
            var query = new QueryBuilder()
                .UseOffsetPaging(new OffsettPaginationConfiguration(2, 2)) // Page 2, page size 2
                .Build();

            // Act
            var result = _executor.Execute(query, data);

            // Assert
            Assert.Equal(5, result.TotalDataCount); // Total unchanged
            Assert.Equal(2, result.Items.Count()); // Page size respected
            Assert.NotNull(result.PageInfo);
            Assert.True(result.PageInfo.HasNextPage);
            Assert.True(result.PageInfo.HasPreviousPage);
        }

        [Fact]
        public void Execute_WithOffsetPaging_FirstPage_ShouldIndicateNoPreviousPage()
        {
            // Arrange
            var data = CreateTestData();
            var query = new QueryBuilder()
                .UseOffsetPaging(new OffsettPaginationConfiguration(1, 2)) // First page, page size 2
                .Build();

            // Act
            var result = _executor.Execute(query, data);

            // Assert
            Assert.NotNull(result.PageInfo);
            Assert.False(result.PageInfo.HasPreviousPage);
            Assert.True(result.PageInfo.HasNextPage);
        }

        [Fact]
        public void Execute_WithOffsetPaging_LastPage_ShouldIndicateNoNextPage()
        {
            // Arrange
            var data = CreateTestData();
            var query = new QueryBuilder()
                .UseOffsetPaging(new OffsettPaginationConfiguration(1, 10)) // Page 1, page size larger than total
                .Build();

            // Act
            var result = _executor.Execute(query, data);

            // Assert
            Assert.NotNull(result.PageInfo);
            Assert.False(result.PageInfo.HasPreviousPage);
            Assert.False(result.PageInfo.HasNextPage);
        }

        [Fact]
        public async Task ExecuteAsync_ShouldReturnSameResultAsExecute()
        {
            // Arrange
            var data = CreateTestData();
            var filters = new FilterConfiguration();
            filters.WithFilters([new FilterDescriptor("Age", FilterOperator.GreaterThan, 20)]);
            var query = new QueryBuilder().WithFilters(filters).Build();

            // Act
            var syncResult = _executor.Execute(query, data);
            var asyncResult = await _executor.ExecuteAsync(query, data);

            // Assert
            Assert.Equal(syncResult.TotalDataCount, asyncResult.TotalDataCount);
            Assert.Equal(syncResult.Items.Count(), asyncResult.Items.Count());
        }

        [Fact]
        public void Execute_WithMultipleFilters_ShouldApplyAllFilters()
        {
            // Arrange
            var data = CreateTestData();
            var filters = new FilterConfiguration();
            filters.WithFilters([
                new FilterDescriptor("Age", FilterOperator.GreaterThan, 20),
                new FilterDescriptor("Name", FilterOperator.Contains, "o")
            ]);
            var query = new QueryBuilder().WithFilters(filters).Build();

            // Act
            var result = _executor.Execute(query, data);

            // Assert
            Assert.All(result.Items, item =>
            {
                Assert.True(item.Age > 20);
                Assert.Contains("o", item.Name, StringComparison.OrdinalIgnoreCase);
            });
        }

        private static IEnumerable<TestEntity> CreateTestData()
        {
            return
            [
                new TestEntity { Id = 1, Name = "Alice", Age = 25, IsActive = true },
                new TestEntity { Id = 2, Name = "Bob", Age = 30, IsActive = false },
                new TestEntity { Id = 3, Name = "Charlie", Age = 35, IsActive = true },
                new TestEntity { Id = 4, Name = "David", Age = 20, IsActive = true },
                new TestEntity { Id = 5, Name = "Eve", Age = 40, IsActive = false }
            ];
        }
    }

    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public bool IsActive { get; set; }
    }
}
