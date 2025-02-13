using Microsoft.Extensions.Logging;

namespace Puzzle;

public static partial class Logging
{
    [LoggerMessage(LogLevel.Debug, "Discovered plugin {Plugin} ({PluginId})")]
    public static partial void DiscoveredPlugin(
        this ILogger logger,
        string plugin,
        string pluginId
    );
}
