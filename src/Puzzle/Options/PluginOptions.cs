namespace Puzzle.Options;

internal sealed record PluginOptions
{
    public bool Disabled { get; init; }
    public int? Priority { get; init; }
}
