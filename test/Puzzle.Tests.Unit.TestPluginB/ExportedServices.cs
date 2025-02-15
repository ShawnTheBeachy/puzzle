using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;
using Puzzle.Tests.Unit.TestPlugin.Abstractions;

namespace Puzzle.Tests.Unit.TestPluginB;

[Service<IService>(Lifetime)]
public sealed class ExportedService : IService
{
    public const ServiceLifetime Lifetime = ServiceLifetime.Scoped;
}
