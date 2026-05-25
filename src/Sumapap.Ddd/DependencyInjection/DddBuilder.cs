using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sumapap.Ddd.DependencyInjection.Abstractions;
using Sumapap.Ddd.DependencyInjection.Strategies;
using Sumapap.DependencyInjection.Abstractions;

namespace Sumapap.Ddd.DependencyInjection
{
    internal class DddBuilder(ISumapapServiceBuilder _builder) : IDddBuilder
    {
        private readonly IServiceCollection _services = _builder.Services;
        private readonly List<EventDispatcherRegistration> _registrations = [];

        private IEventDispatcherRegistrationStrategy _strategy = new DefaultEventDispatcherRegistrationStrategy();

        IServiceCollection IBuilder<ISumapapServiceBuilder>.Services => _services;

        public ISumapapServiceBuilder Build()
        {
            foreach (var registration in _registrations)
            {
                _strategy.Register(registration, _services);
            }

            return _builder;
        }

        public EventDispatcherRegistrationConfigurator AddDomainEventsDispatcher(Assembly[] assemblies)
        {
            _registrations.Add(new EventDispatcherRegistration(assemblies));

            return new EventDispatcherRegistrationConfigurator(this);
        }

        public IDddBuilder SetStrategy(IEventDispatcherRegistrationStrategy strategy)
        {
            _strategy = strategy;

            return this;
        }
    }
}
