using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.Caching.FusionCache.Repositories;
using Sumapap.Persistence.DependencyInjection;
using Sumapap.Persistence.DependencyInjection.Abstractions;
using Sumapap.Persistence.Extensions;

namespace Sumapap.Persistence.Caching.FusionCache.Visitors
{
    internal class RepositoryDecorationVisitor : IRepositoryRegistrationVisitor
    {
        public void Visit(RepositoryRegistration entry, IServiceCollection services)
        {
            if (entry.AllowCaching)
            {
                var cachedImplType = GetCachedImplType(entry.ImplType);
                var repositoryInterfaces = entry.ImplType.GetRepositoryInterfacesTypes();

                // decorate all repository interfaces with the corresponding cached implementation, except the AbstractType.
                foreach (var interf in repositoryInterfaces)
                {
                    services.TryDecorate(interf, cachedImplType);
                }
            }
        }

        private static Type GetCachedImplType(Type implType)
        {
            var rwInterface = implType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IReadWriteRepository<,>));
            var roInterface = implType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IReadRepository<,>));
            var woInterface = implType.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IWriteRepository<,>));

            if (rwInterface == null &&
                roInterface == null &&
                woInterface == null)
            {
                throw new InvalidOperationException("Repository should implements the corresponding interface");
            }

            if (rwInterface != null)
            {
                return implType.IsGenericTypeDefinition 
                    ? typeof(CachedReadWriteRepository<,>)
                    : typeof(CachedReadWriteRepository<,>).MakeGenericType(rwInterface.GenericTypeArguments);
            }

            if (roInterface != null)
            {
                return implType.IsGenericTypeDefinition
                    ? typeof(CachedReadRepository<,>)
                    : typeof(CachedReadRepository<,>).MakeGenericType(roInterface.GenericTypeArguments);
            }

            if (woInterface != null)
            {
                return implType.IsGenericTypeDefinition 
                    ? typeof(CachedWriteRepository<,>)
                    : typeof(CachedWriteRepository<,>).MakeGenericType(woInterface.GenericTypeArguments);
            }

            throw new InvalidOperationException();
        }
    }
}
