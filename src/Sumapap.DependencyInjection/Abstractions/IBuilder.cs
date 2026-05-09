namespace Sumapap.DependencyInjection.Abstractions
{
    public interface IBuilder<T>
    {
        T Build();
    }
}
