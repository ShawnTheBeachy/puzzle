using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Puzzle;

public sealed class PuzzleConfiguration
{
    public IReadOnlyList<Plugin> Plugins { get; }
    public IConfiguration Configuration { get; }
    public IServiceCollection Services { get; }

    public PuzzleConfiguration(
        IReadOnlyList<Plugin> plugins,
        IConfiguration configuration,
        IServiceCollection services
    )
    {
        Plugins = plugins;
        Configuration = configuration;
        Services = services;
    }
}
