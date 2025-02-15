using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Puzzle.Abstractions;
using Puzzle.Tests.Unit.TestPlugin.Abstractions;

namespace Puzzle.Tests.Unit.TestPlugin;

[Service<IDependentService>(ServiceLifetime.Transient)]
public sealed class ExportedDependentService : IDependentService
{
    public ExportedDependentService(IDependentService.IDependency dependency)
    {
        _ = dependency;
    }
}

[Service<IExclusiveService>(Lifetime)]
public sealed class ExportedExclusiveService : IExclusiveService
{
    public const ServiceLifetime Lifetime = ServiceLifetime.Singleton;
}

[Service<IHostedService>(ServiceLifetime.Transient)]
public sealed class ExportedHostedService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

[Service<IService>(Lifetime)]
public sealed class ExportedService : IService
{
    public const ServiceLifetime Lifetime = ServiceLifetime.Transient;
}

public sealed class ExportedServiceWithoutAttribute : IServiceWithoutAttribute;
