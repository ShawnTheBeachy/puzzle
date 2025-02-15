using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;
using Puzzle.Tests.Unit.TestPlugin.Abstractions;

namespace Puzzle.Tests.Unit.TestPlugin;

[Service<IExclusiveService>(Lifetime)]
public sealed class ExportedExclusiveService : IExclusiveService
{
    public const ServiceLifetime Lifetime = ServiceLifetime.Singleton;
}

[Service<IService>(Lifetime)]
public sealed class ExportedService : IService
{
    public const ServiceLifetime Lifetime = ServiceLifetime.Transient;
}

[Service<IDependentService>(ServiceLifetime.Transient)]
public sealed class ExportedDependentService : IDependentService
{
    public ExportedDependentService(IDependentService.IDependency dependency)
    {
        _ = dependency;
    }
}

public sealed class ExportedServiceWithoutAttribute : IServiceWithoutAttribute;
