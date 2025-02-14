namespace Puzzle.Options;

internal sealed record PuzzleOptions
{
    public IReadOnlyList<string> Locations { get; init; } = [];
    public const string SectionName = "Plugins";
    public TimeSpan? StartupThreshold { get; init; }
}
