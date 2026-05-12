using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Persistence.Abstractions;

namespace Sumapap.Persistence.DependencyInjection
{
    /// <summary>
    /// Extension methods for configuring Sumapap Persistence repositories.
    /// </summary>
    public static class SumapapServiceBuilderExtensions
    {
        /// <summary>
        /// Configures repository registrations with fluent API supporting opt-in caching.
        /// </summary>
        /// <param name="configuration">Action to configure repository registrations.</param>
        /// <returns>The same builder for method chaining.</returns>
        /// <example>
        /// <code>
        /// services.AddSumapap()
        ///     .WithRepositories(builder =>
        ///     {
        ///         builder.AddScopedRepository&lt;UserRepository, User&gt;()
        ///             .AllowCaching(config =>
        ///             {
        ///                 config[nameof(IReadRepository&lt;User&gt;.FindAsync)] = true;
        ///                 config[nameof(IReadRepository&lt;User&gt;.GetAllAsync)] = true;
        ///             });
        ///             
        ///         builder.AddScopedRepository&lt;ProductRepository, Product&gt;();  // No caching
        ///     });
        /// </code>
        /// </example>
        extension(ISumapapServiceBuilder builder)
        {
            public ISumapapServiceBuilder WithRepositories(Action<IRepositoryRegistrationBuilder> configuration)
            {
                ArgumentNullException.ThrowIfNull(configuration);

                var repositoryBuilder = new RepositoryRegistrationBuilder(builder);

                configuration(repositoryBuilder);

                return repositoryBuilder.Build();
            }
        }
    }
}
