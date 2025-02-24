using Puzzle.Options;

namespace Puzzle.Tests.Unit.Options;

public sealed class PuzzleOptionsTests
{
    [Test]
    public async Task NewOptions_ShouldBeInitializedWithReasonableDefaultValues_WhenValuesAreNotSet()
    {
        // Act.
        var options = new PuzzleOptions();

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(options.IsolatePlugins).IsTrue();
        await Assert.That(options.Locations).IsEmpty();
        await Assert.That(options.StartupThreshold).IsNull();
    }
}
