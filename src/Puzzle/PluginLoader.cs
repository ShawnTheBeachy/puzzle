using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Puzzle.Abstractions;
using Puzzle.Bootstrap;
using Puzzle.Extensions;
using Puzzle.Options;

namespace Puzzle;

internal sealed class PluginLoader : IPluginLoader
{
    private readonly List<Plugin> _plugins = [];
    private readonly IServiceProvider _serviceProvider;
    public TimeSpan? StartupThreshold { get; }

    public PluginLoader(
        IConfiguration configuration,
        ILogger<PluginLoader> logger,
        IServiceProvider serviceProvider
    )
    {
        _serviceProvider = serviceProvider;
        var options = configuration.Get<PuzzleOptions>();

        if (options is null)
            return;

        StartupThreshold = options.StartupThreshold;
        var pluginAssemblies = AssemblyScanning.ScanAssemblies(options, logger);

        foreach (var assembly in pluginAssemblies)
        {
            if (!Plugin.TryCreate(assembly, configuration, out var plugin) || plugin is null)
                continue;

            _plugins.Add(plugin);
            logger.DiscoveredPlugin(plugin.Name, plugin.Id);
        }
    }

    private IEnumerable<Plugin> EligiblePlugins(string? pluginId) =>
        pluginId is null ? _plugins : _plugins.Where(x => x.Id == pluginId);

    public TService? GetService<TService>(string? pluginId = null)
        where TService : class
    {
        foreach (var plugin in EligiblePlugins(pluginId).OrderBy(x => x.Priority))
        {
            foreach (var type in plugin.AllTypes.GetTypes())
            {
                if (!IsService<TService>(type))
                    continue;

                var services = new ServiceCollection().AddTransient(type, type);

                var provider = plugin.Bootstrap(services, _serviceProvider);
                return (TService)provider.GetRequiredService(type);
            }
        }

        return null;
    }

    private static bool IsService<TService>(Type type)
    {
        if (!type.IsAssignableTo(typeof(TService)))
            return false;

        if (!type.TryFindService(out var serviceType, out _, out _))
            return false;

        return serviceType == typeof(TService);
    }

    public IReadOnlyList<Plugin> Plugins() => _plugins.ToArray();
}
