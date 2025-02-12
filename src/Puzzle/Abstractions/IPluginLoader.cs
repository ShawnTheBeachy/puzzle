namespace Puzzle.Abstractions;

public interface IPluginLoader
{
    IReadOnlyList<Plugin> Plugins();
}
