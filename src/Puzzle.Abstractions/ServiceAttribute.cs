using Microsoft.Extensions.DependencyInjection;

namespace Puzzle.Abstractions;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceAttribute(ServiceLifetime? lifetime) : Attribute
{
    public ServiceLifetime? Lifetime => lifetime;
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class ServiceAttribute<TService> : ServiceAttribute
    where TService : class
{
    public ServiceAttribute()
        : base(null) { }

    public ServiceAttribute(ServiceLifetime lifetime)
        : base(lifetime) { }
}
