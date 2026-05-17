using Sumapap.Persistence.Abstractions;

namespace Sumapap.Persistence.DependencyInjection
{
    /// <summary>
    /// Fluent configurator for individual repository registration, enabling opt-in caching.
    /// </summary>
    public sealed class RepositoryConfigurator<TRepository, TEntity> : RepositoryConfigurator
        where TRepository : class
        where TEntity : class, IEntity
    {
        internal RepositoryConfigurator(RepositoryRegistrationBuilder builder, int registrationIndex)
            : base(builder, registrationIndex)
        {
        }
    }

    /// <summary>
    /// Fluent configurator for repository registration, enabling opt-in caching.
    /// Supports both generic (open) and non-generic (closed) repository types.
    /// </summary>
    public class RepositoryConfigurator
    {
        private readonly RepositoryRegistrationBuilder _builder;
        private readonly int _registrationIndex;

        internal RepositoryConfigurator(RepositoryRegistrationBuilder builder, int registrationIndex)
        {
            _builder = builder;
            _registrationIndex = registrationIndex;
        }

        internal IRepositoryRegistrationBuilder Builder => _builder;

        internal int RegistrationIndex => _registrationIndex;
    }
}
