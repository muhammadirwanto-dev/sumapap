using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sumapap.Ddd.Abstractions.Events;
using Sumapap.Ddd.Mediator.Events;

namespace Sumapap.Ddd.Mediator.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a domain event dispatcher to the service collection for handling domain events within the application.
        /// You have to register your Mediator instance in order to use the DomainEventDispatcher. For example, if you are using MediatR, you can do it like this:
        ///
        ///  .AddMediator(options =>
        ///   {
        ///      options.Assemblies = [typeof(DependencyInjection)];
        ///      options.PipelineBehaviors = [typeof(ValidationBehavior<,>)];
        ///   });             
        ///
        /// Also, add the following line to your .csproj:
        ///  <PackageReference Include="Mediator.Abstractions" Version="3.0.*"/>
        ///  <PackageReference Include="Mediator.SourceGenerator" Version = "3.0.*">
        ///      <PrivateAssets> all </PrivateAssets>
        ///      <IncludeAssets> runtime; build; native; contentfiles; analyzers </IncludeAssets>
        ///  </PackageReference>
        /// </summary>
        /// <param name="services">The service collection to which the domain event dispatcher will be added. Cannot be null.</param>
        /// <returns>The service collection with the domain event dispatcher registered. This enables domain event dispatching
        /// capabilities in the application's dependency injection container.</returns>
        public static IServiceCollection AddMediatorEventsDispatcher(
            this IServiceCollection services)
        {
            return services
                .AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
        }
    }
}
