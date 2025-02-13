using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Puzzle.Abstractions;

namespace Puzzle.Tests.Unit.TestPlugin;

[Service<IHostedService>(ServiceLifetime.Transient)]
public sealed class ExportedHostedService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
