using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;

namespace Puzzle.Tests.Unit.TestPlugin;

public sealed class ExportedBootstrapper : IPluginBootstrapper
{
    public IConfiguration? Configuration { get; set; }
    public IServiceCollection? Services { get; set; }

    public IServiceCollection Bootstrap(IServiceCollection services, IConfiguration configuration)
    {
        Configuration = configuration;
        Services = services;
        services.AddSingleton<IPluginBootstrapper>(this);
        return services;
    }
}
