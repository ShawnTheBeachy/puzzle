using Puzzle.Abstractions;

namespace Puzzle.Tests.Unit.TestPluginA;

public sealed class ExportedMetadata : IPluginMetadata
{
    public string Id => "com.tests.a";
    public string Name => "Tests A";
}
