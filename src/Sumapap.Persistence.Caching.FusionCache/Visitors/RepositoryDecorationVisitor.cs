using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.Caching.FusionCache.Repositories;
using Sumapap.Persistence.DependencyInjection;

namespace Sumapap.Persistence.Caching.FusionCache.Visitors
{
    internal class RepositoryDecorationVisitor : IRepositoryRegistrationVisitor
    {
        public void Visit(RepositoryRegistrationEntry entry, IServiceCollection services)
        {
            if (entry.AllowCaching)
            {
                services.Decorate(entry.AbstractType, GetCachedImplType(entry.ImplType));
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
                return typeof(CachedReadWriteRepository<,>);
            }

            if (roInterface != null)
            {
                return typeof(CachedReadRepository<,>);
            }

            if (woInterface != null)
            {
                return typeof(CachedWriteRepository<,>);
            }

            throw new InvalidOperationException();
        }
    }
}
