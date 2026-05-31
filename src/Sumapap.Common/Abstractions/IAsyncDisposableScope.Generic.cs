namespace Sumapap.Common.Abstractions
{
    public interface IAsyncDisposableScope<T> : IAsyncDisposableScope
    {
        T Context { get; }
    }
}
