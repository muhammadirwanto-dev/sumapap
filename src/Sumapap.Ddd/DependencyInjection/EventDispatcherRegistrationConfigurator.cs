using Sumapap.Ddd.DependencyInjection.Abstractions;

namespace Sumapap.Ddd.DependencyInjection
{
    public class EventDispatcherRegistrationConfigurator
    {
        public IDddBuilder Builder { get; }

        internal EventDispatcherRegistrationConfigurator(IDddBuilder dddBuilder)
        {
            Builder = dddBuilder;
        }
    }
}
