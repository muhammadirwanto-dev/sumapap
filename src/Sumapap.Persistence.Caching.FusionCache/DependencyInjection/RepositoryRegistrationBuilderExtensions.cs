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
                    if (entry.IsGeneric)
                    {
                        builder.Services.Decorate(entry.AbstractType, GetCachedImplType(entry.ImplType));
                    }
                }
            }

            private static Type GetCachedImplType(Type implType)
            {
                if (implType.IsGenericType)
                {
                    var typeArguments = implType.GetGenericArguments();
                    var genericDefinition = implType.GetGenericTypeDefinition();

                    if (genericDefinition == typeof(IReadRepository<,>))
                    {
                        return typeof(CachedReadRepository<,>).MakeGenericType(typeArguments);
                    }
                    else if (genericDefinition == typeof(IWriteRepository<,>))
                    {
                        //return typeof(CachedWriteRepository<,>).MakeGenericType(typeArguments);
                    }
                }

                throw new NotSupportedException($"Unsupported repository type: {implType.FullName}");
            }
        }
    }
}
