using Microsoft.Extensions.DependencyInjection;
using Sumapap.Persistence.Abstractions;
using Sumapap.Persistence.Caching.FusionCache.Repositories;
using Sumapap.Persistence.Caching.Visitors;
using Sumapap.Persistence.DependencyInjection;

namespace Sumapap.Persistence.Caching.FusionCache.DependencyInjection
{
    public static class RepositoryRegistrationBuilderExtensions
    {
        extension(IRepositoryRegistrationBuilder builder)
        {
            /// <summary>
            /// Registers generic repositories with FusionCache caching enabled for all methods.
            /// </summary>
            /// <param name="serviceLifetime">The lifetime for the repository services.</param>
            /// <returns>The same builder for method chaining.</returns>
            public IServiceCollection UseCacheProvider()
            {
                builder
                    .AddVisitor(new CachedRepositoryVisitor())
                    .DecorateRepositories(builder.Registrations);

                return builder.Services;
            }

            private void DecorateRepositories(IEnumerable<RepositoryRegistrationEntry> entries)
            {
                foreach (var entry in entries)
                {
                    if (entry.AllowCaching)
                    {
                        builder.Services.Decorate(entry.AbstractType, GetCachedImplType(entry.ImplType));
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
                    var typeArguments = rwInterface.GetGenericArguments();
                    return typeof(CachedReadWriteRepository<,>).MakeGenericType(typeArguments);
                }

                if (roInterface != null)
                {
                    var typeArguments = roInterface.GetGenericArguments();
                    return typeof(CachedReadRepository<,>).MakeGenericType(typeArguments);
                }

                if (woInterface != null)
                {
                    var typeArguments = woInterface.GetGenericArguments();
                    return typeof(CachedWriteRepository<,>).MakeGenericType(typeArguments);
                }

                throw new InvalidOperationException();
            }
        }
    }
}
