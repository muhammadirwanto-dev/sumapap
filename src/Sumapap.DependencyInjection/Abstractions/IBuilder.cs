using Microsoft.Extensions.DependencyInjection;

namespace Sumapap.DependencyInjection.Abstractions
{
    public interface IBuilder<T>
    {
        T Build();

        internal IServiceCollection Services { get; }
    }
}
