namespace Sumapap.Common.Abstractions
{
    public interface IDisposableScope<T> : IDisposableScope
    {
        T Context { get; }
    }
}
