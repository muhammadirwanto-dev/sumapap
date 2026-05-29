using Sumapap.Persistence.DependencyInjection.Abstractions;

namespace Sumapap.Persistence.DependencyInjection
{
    /// <summary>
    /// Fluent configurator for repository registration, enabling opt-in caching.
    /// Supports both generic (open) and non-generic (closed) repository types.
    /// </summary>
    public class RepositoryConfigurator
    {
        private readonly IPersistenceBuilder _builder;
        private readonly int _registrationIndex;

        internal RepositoryConfigurator(IPersistenceBuilder builder, int registrationIndex)
        {
            _builder = builder;
            _registrationIndex = registrationIndex;
        }

        internal IPersistenceBuilder Builder => _builder;

        internal int RegistrationIndex => _registrationIndex;
    }
}
