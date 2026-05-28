using Microsoft.Extensions.DependencyInjection;
using Sumapap.DependencyInjection.Abstractions;
using Sumapap.Reporting.Abstractions;
using Sumapap.Reporting.DependencyInjection.Abstractions;
using Sumapap.Reporting.Options;

namespace Sumapap.Reporting.DependencyInjection
{
    internal class ReportingBuilder(ISumapapServiceBuilder _builder) : IReportingBuilder
    {
        private readonly IServiceCollection _services = _builder.Services;

        IServiceCollection IBuilder<ISumapapServiceBuilder>.Services => _services;

        public SinksConfigurator Sinks { get; } = new SinksConfigurator();

        public IReportingBuilder AddReporting() => AddReporting(_ => { });

        public IReportingBuilder AddReporting(Action<ReportingOptions> configuration)
        {
            _services.Configure(configuration);
            _services.AddSingleton<IReporter, Reporter>();

            return this;
        }

        public ISumapapServiceBuilder Build()
        {
            return _builder;
        }
    }
}
