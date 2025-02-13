using Puzzle.Abstractions;

namespace Puzzle.Tests.Unit;

public sealed class ExportedMetadata : IPluginMetadata
{
    public string Id => "com.tests";
    public string Name => "Tests";
}
