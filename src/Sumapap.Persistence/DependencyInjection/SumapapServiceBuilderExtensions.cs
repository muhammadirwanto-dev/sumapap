using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Persistence.DependencyInjection.Abstractions;

namespace Sumapap.Persistence.DependencyInjection
{
    /// <summary>
    /// Extension methods for configuring Sumapap Persistence repositories.
    /// </summary>
    public static class SumapapServiceBuilderExtensions
    {
        public static ISumapapServiceBuilder WithPersistence(this ISumapapServiceBuilder builder, Action<IPersistenceBuilder> configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);

            var persistenceBuilder = new PersistenceBuilder(builder);

            configuration(persistenceBuilder);

            return persistenceBuilder.Build();
        }
    }
}
