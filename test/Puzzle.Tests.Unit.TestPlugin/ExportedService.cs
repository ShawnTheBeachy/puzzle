using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;

namespace Puzzle.Tests.Unit.TestPlugin;

[Service<ITuple>(Lifetime)]
public sealed class ExportedService : ITuple
{
    public const ServiceLifetime Lifetime = ServiceLifetime.Transient;

    public object this[int index] => this;

    public int Length => 1;
}

[Service<ICloneable>(ServiceLifetime.Transient)]
public sealed class ExportedDependentService : ICloneable
{
    public ExportedDependentService(IFormatProvider provider)
    {
        _ = provider;
    }

    public object Clone() => this;
}

public sealed class ExportedServiceWithoutAttribute : IFormatProvider
{
    public object? GetFormat(Type? formatType) => null;
}
