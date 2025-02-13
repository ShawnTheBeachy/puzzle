using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;

namespace Puzzle.Tests.Unit.TestPlugin;

public sealed class ExportedBootstrapper : IPluginBootstrapper
{
    public IServiceCollection Bootstrap(
        IServiceCollection services,
        IConfiguration configuration
    ) => services;
}
