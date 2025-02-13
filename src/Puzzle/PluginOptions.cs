namespace Puzzle;

internal sealed record PluginOptions
{
    public IReadOnlyList<string> Locations { get; init; } = [];
    public const string SectionName = "Plugins";
    public TimeSpan? StartupThreshold { get; init; }
}
