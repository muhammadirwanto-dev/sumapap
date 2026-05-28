using Sumapap.Reporting.DependencyInjection.Abstractions;

namespace Sumapap.Reporting.DependencyInjection
{
    public struct SinksConfigurator(IReportingBuilder _builder)
    {
        public readonly IReportingBuilder Builder => _builder;
    }
}