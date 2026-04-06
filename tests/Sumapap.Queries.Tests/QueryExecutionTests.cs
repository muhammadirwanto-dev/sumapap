using Sumapap.Queries.Execution.Executors;
using Sumapap.Queries.Filtering;
using Sumapap.Queries.Paging;
using Sumapap.Queries.Sorting;

namespace Sumapap.Queries.Tests
{
    public class QueryExecutionTests
    {
        private record Person(int Id, string Name, int Age);

        [Fact]
        public void EnumerableExecutor_Filters_Equals()
        {
            var data = new List<Person>
            {
                new(1, "Alice", 30),
                new(2, "Bob", 25),
                new(3, "Alice", 40)
            };

            var query = new Query();
            var executor = new EnumerableQueryExecutor<Person>();

            query.Filters.RootGroup.WithFilters(
            [
                new("Name", FilterOperator.Equals, "Alice")
            ]);

            var result = executor.Execute(query, data);

            Assert.Equal(2, result.Items.Count());
            Assert.All(result.Items, i => Assert.Equal("Alice", i.Name));
        }

        [Fact]
        public void EnumerableExecutor_Filters_CompositeOr()
        {
            var data = new List<Person>
            {
                new(1, "Alice", 30),
                new(2, "Bob", 25),
                new(3, "Charlie", 40)
            };

            var query = new Query();
            var executor = new EnumerableQueryExecutor<Person>();

            query.Filters.RootGroup.WithOperator(CompositeOperator.Or).WithFilters(
            [
                new("Name", FilterOperator.Equals, "Bob"),
                new("Age", FilterOperator.GreaterThan, 35)
            ]);

            var result = executor.Execute(query, data);

            Assert.Equal(2, result.Items.Count());
            Assert.Contains(result.Items, i => i.Name == "Bob");
            Assert.Contains(result.Items, i => i.Name == "Charlie");
        }

        [Fact]
        public void EnumerableExecutor_Sorts_And_MultipleSorts()
        {
            var data = new List<Person>
            {
                new(1, "Alice", 30),
                new(2, "Bob", 30),
                new(3, "Charlie", 25)
            };

            var query = new Query();
            var executor = new EnumerableQueryExecutor<Person>();

            query.Sort
                .By("Age", SortDirection.Asc)
                .ThenBy("Name", SortDirection.Desc);

            var result = executor.Execute(query, data);

            Assert.Equal(3, result.Items.Count());
            // Ages: 25,30,30
            Assert.Equal(25, result.Items.ElementAt(0).Age);
            Assert.Equal(30, result.Items.ElementAt(1).Age);
            Assert.Equal(30, result.Items.ElementAt(2).Age);
            // For same Age, Name desc -> Bob before Alice
            Assert.Equal("Charlie", result.Items.ElementAt(0).Name);
            Assert.Equal("Bob", result.Items.ElementAt(1).Name);
            Assert.Equal("Alice", result.Items.ElementAt(2).Name);
        }

        [Fact]
        public void EnumerableExecutor_OffsetPaging()
        {
            var data = Enumerable.Range(1, 10).Select(i => new Person(i, $"N{i}", i)).ToList();

            var query = new Query(new OffsetPaginationOptions(page: 2, pageSize: 3));
            var executor = new EnumerableQueryExecutor<Person>();

            var result = executor.Execute(query, data);

            Assert.NotNull(result.PageInfo);
            Assert.Equal(3, result.Items.Count());
            Assert.Equal(10, result.TotalDataCount);
            // Items should be 4,5,6
            Assert.Equal(4, result.Items.ElementAt(0).Id);
            Assert.Equal(5, result.Items.ElementAt(1).Id);
            Assert.Equal(6, result.Items.ElementAt(2).Id);
        }

        [Fact]
        public void EnumerableExecutor_CursorPaging_Basic()
        {
            var data = Enumerable.Range(1, 5).Select(i => new Person(i, $"N{i}", i)).ToList();

            var query = new Query(new CursorPaginationOptions("Id", null, limit: 2, direction: CursorDirection.Forward));
            var executor = new EnumerableQueryExecutor<Person>();

            var result = executor.Execute(query, data);

            Assert.NotNull(result.PageInfo);
            Assert.Equal(2, result.Items.Count());
            Assert.True(result.PageInfo.HasNextPage);
            Assert.False(result.PageInfo.HasPreviousPage);
            Assert.NotNull(result.PageInfo.EndCursor);
        }

        [Fact]
        public void EnumerableExecutor_EmptyFilters_ReturnsAll()
        {
            var data = new List<Person>
            {
                new(1, "A", 10),
                new(2, "B", 20)
            };

            var query = new Query();
            var executor = new EnumerableQueryExecutor<Person>();

            var result = executor.Execute(query, data);

            Assert.Equal(2, result.Items.Count());
        }
    }
}
