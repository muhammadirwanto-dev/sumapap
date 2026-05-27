using Sumapap.Persistence.Abstractions.Specifications;
using Sumapap.Queries;
using Sumapap.Queries.Abstractions.Filtering;
using System.Linq.Expressions;
using Xunit;

namespace Sumapap.Persistence.Abstractions.Tests.Specifications
{
    public class SpecificationTests
    {
        private class TestEntity
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
        }

        private class TestSpecification : ISpecification<TestEntity>
        {
            public Expression<Func<TestEntity, bool>>? Criteria { get; set; }
            public IList<string> Includes { get; set; } = new List<string>();
            public Sumapap.Queries.Abstractions.IQuery? QueryOptions { get; set; }
        }

        [Fact]
        public void Specification_WithCriteria_ShouldSetExpression()
        {
            // Arrange
            Expression<Func<TestEntity, bool>> criteria = x => x.Age > 18;
            var spec = new TestSpecification { Criteria = criteria };

            // Act & Assert
            Assert.NotNull(spec.Criteria);
            Assert.Equal(criteria, spec.Criteria);
        }

        [Fact]
        public void Specification_WithIncludes_ShouldStoreIncludePaths()
        {
            // Arrange
            var spec = new TestSpecification
            {
                Includes = ["Orders", "Orders.OrderItems"]
            };

            // Act & Assert
            Assert.Equal(2, spec.Includes.Count);
            Assert.Contains("Orders", spec.Includes);
            Assert.Contains("Orders.OrderItems", spec.Includes);
        }

        [Fact]
        public void Specification_WithQueryOptions_ShouldStoreQuery()
        {
            // Arrange
            var query = new QueryBuilder()
                .UseOffsetPaging(10, 0)
                .Build();
            var spec = new TestSpecification { QueryOptions = query };

            // Act & Assert
            Assert.NotNull(spec.QueryOptions);
            Assert.True(spec.QueryOptions.UsesOffsetPaging);
        }

        [Fact]
        public void Specification_CanBeEmpty_WithAllPropertiesNull()
        {
            // Arrange & Act
            var spec = new TestSpecification();

            // Assert
            Assert.Null(spec.Criteria);
            Assert.Empty(spec.Includes);
            Assert.Null(spec.QueryOptions);
        }

        [Fact]
        public void Specification_WithComplexCriteria_ShouldCompileCorrectly()
        {
            // Arrange
            Expression<Func<TestEntity, bool>> criteria = x => x.Age >= 18 && x.Name.StartsWith("A");
            var spec = new TestSpecification { Criteria = criteria };
            var testData = new[]
            {
                new TestEntity { Id = 1, Name = "Alice", Age = 25 },
                new TestEntity { Id = 2, Name = "Bob", Age = 30 },
                new TestEntity { Id = 3, Name = "Andrew", Age = 20 }
            };

            // Act
            var compiled = spec.Criteria!.Compile();
            var filtered = testData.Where(compiled).ToList();

            // Assert
            Assert.Equal(2, filtered.Count);
            Assert.Contains(filtered, x => x.Name == "Alice");
            Assert.Contains(filtered, x => x.Name == "Andrew");
        }

        [Fact]
        public void Specification_WithNestedIncludes_ShouldHandleMultipleLevels()
        {
            // Arrange
            var spec = new TestSpecification
            {
                Includes = ["Parent", "Parent.Children", "Parent.Children.GrandChildren"]
            };

            // Act & Assert
            Assert.Equal(3, spec.Includes.Count);
            Assert.All(spec.Includes, include => Assert.NotEmpty(include));
        }

        [Fact]
        public void Specification_WithQueryAndCriteria_ShouldAllowBothTogether()
        {
            // Arrange
            Expression<Func<TestEntity, bool>> criteria = x => x.Age > 21;
            var filters = new FilterConfiguration();
            filters.WithFilters([new FilterDescriptor("Name", FilterOperator.Contains, "Test")]);
            var query = new QueryBuilder().WithFilters(filters).Build();

            var spec = new TestSpecification
            {
                Criteria = criteria,
                QueryOptions = query
            };

            // Act & Assert
            Assert.NotNull(spec.Criteria);
            Assert.NotNull(spec.QueryOptions);
            Assert.NotEmpty(spec.QueryOptions.Filters.Filters);
        }
    }
}
