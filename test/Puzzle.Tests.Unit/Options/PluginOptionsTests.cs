using Puzzle.Options;

namespace Puzzle.Tests.Unit.Options;

public sealed class PluginOptionsTests
{
    [Test]
    public async Task NewOptions_ShouldBeInitializedWithReasonableDefaultValues_WhenValuesAreNotSet()
    {
        // Act.
        var options = new PluginOptions();

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(options.Disabled).IsFalse();
        await Assert.That(options.Priority).IsNull();
    }
}
