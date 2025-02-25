namespace Puzzle.Abstractions;

public interface IPluginLoader
{
    TService? GetService<TService>(string? pluginId = null)
        where TService : class;
    IReadOnlyList<Plugin> Plugins();
}
