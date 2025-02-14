using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Puzzle.Abstractions;

namespace Puzzle;

internal sealed class PluginLoader : IPluginLoader
{
    private readonly List<Plugin> _plugins = [];
    public TimeSpan? StartupThreshold { get; }

    public PluginLoader(IConfiguration configuration, ILogger<PluginLoader> logger)
    {
        var options = configuration.Get<PuzzleOptions>();

        if (options is null)
            return;

        StartupThreshold = options.StartupThreshold;
        var pluginAssemblies = AssemblyScanning.ScanAssemblies(options);

        foreach (var assembly in pluginAssemblies)
        {
            if (!Plugin.TryCreate(assembly, configuration, out var plugin) || plugin is null)
                continue;

            _plugins.Add(plugin);
            logger.DiscoveredPlugin(plugin.Name, plugin.Id);
        }
    }

    public IReadOnlyList<Plugin> Plugins() => _plugins.ToArray();
}
