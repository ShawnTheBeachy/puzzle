using Puzzle.Abstractions;

namespace Puzzle.Tests.Unit.TestPluginB;

public sealed class ExportedMetadata : IPluginMetadata
{
    public string Id => "com.tests.b";
    public string Name => "Tests B";
}
