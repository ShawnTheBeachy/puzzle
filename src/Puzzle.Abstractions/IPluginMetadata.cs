namespace Puzzle.Abstractions;

/// <summary>
/// Metadata which provides information about a plugin. Every plugin must expose one class which implements this interface.
/// </summary>
public interface IPluginMetadata
{
    /// <summary>
    /// The id of the plugin which will be used internally for tracking settings, etc.
    /// Recommended format: com.company.plugin
    /// </summary>
    string Id { get; }

    /// <summary>
    /// The user-friendly name of the plugin.
    /// </summary>
    string Name { get; }
}
