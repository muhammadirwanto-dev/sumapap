namespace Sumapap.Queries.Filtering
{
    public sealed class FilterDescriptor(string field, FilterOperator @operator, object? value = null)
    {
        public string Field { get; } = field;

        public FilterOperator Operator { get; } = @operator;

        public object? Value { get; } = value;
    }
}
