using System.Reflection;

namespace Sumapap.Ddd.DependencyInjection
{
    public sealed record EventDispatcherRegistration(
        Assembly[] Assemblies);
}
