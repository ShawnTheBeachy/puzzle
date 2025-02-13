using Microsoft.Extensions.Logging;

namespace Puzzle;

internal static partial class Logging
{
    [LoggerMessage(LogLevel.Information, Messages.DiscoveredPlugin)]
    public static partial void DiscoveredPlugin(
        this ILogger logger,
        string plugin,
        string pluginId
    );

    [LoggerMessage(LogLevel.Warning, Messages.StartupThresholdWarning)]
    public static partial void StartupThresholdWarning(this ILogger logger, TimeSpan elapsed);

    public static partial class Messages
    {
        public const string DiscoveredPlugin = "Discovered plugin {Plugin} ({PluginId})";
        public const string StartupThresholdWarning = "Plugin initialization took {Elapsed}";
    }
}
