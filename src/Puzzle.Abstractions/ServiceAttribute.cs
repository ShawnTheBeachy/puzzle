using Microsoft.Extensions.DependencyInjection;

namespace Puzzle.Abstractions;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute(ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime => lifetime;
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class ServiceAttribute<TService>(ServiceLifetime lifetime)
    : ServiceAttribute(lifetime)
    where TService : class;
