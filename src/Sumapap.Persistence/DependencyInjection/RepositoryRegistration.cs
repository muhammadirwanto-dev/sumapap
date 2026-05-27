using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.DependencyInjection.Abstractions;

namespace Sumapap.Persistence.DependencyInjection
{
    /// <summary>
    /// Represents a repository service registration configuration.
    /// Supports both generic (open) and non-generic (closed) repository types.
    /// </summary>
    public sealed record RepositoryRegistration(
        ServiceLifetime ServiceLifetime,
        Type AbstractType,
        Type ImplType,
        bool IsGeneric,
        bool AllowCaching,
        IRepositoryRegistrationDecorator? Decorator)
    {
        public void Accept(IRepositoryRegistrationVisitor visitor, IServiceCollection services)
        {
            visitor.Visit(this, services);
        }
    }
}
