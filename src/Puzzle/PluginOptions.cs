namespace Puzzle;

internal sealed record PluginOptions
{
    public required IReadOnlyList<string> Locations { get; init; }
    public const string SectionName = "Plugins";
    public TimeSpan? StartupThreshold { get; init; }
}
