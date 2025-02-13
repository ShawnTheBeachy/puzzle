using Microsoft.Extensions.Logging;
using Puzzle.Abstractions;

namespace Puzzle;

internal sealed class PluginLoader : IPluginLoader
{
    private readonly List<Plugin> _plugins = [];

    public PluginLoader(PluginOptions? options, ILogger<PluginLoader> logger)
    {
        if (options is null)
            return;

        var pluginAssemblies = AssemblyScanning.ScanAssemblies(options);

        foreach (var assembly in pluginAssemblies)
        {
            if (!Plugin.TryCreate(assembly, out var plugin) || plugin is null)
                continue;

            _plugins.Add(plugin);
            logger.DiscoveredPlugin(plugin.Name, plugin.Id);
        }
    }

    public IReadOnlyList<Plugin> Plugins() => _plugins.ToArray();
}
