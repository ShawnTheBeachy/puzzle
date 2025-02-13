using Microsoft.Extensions.Logging;
using Puzzle.Tests.Unit.TestPlugin;

namespace Puzzle.Tests.Unit;

public sealed class PluginLoaderTests
{
    [Test]
    public async Task Plugin_ShouldBeLoaded_WhenItExistsInLocationSpecifiedInOptions()
    {
        // Arrange.
        var options = new PluginOptions { Locations = [GlobalHooks.PluginsPath] };

        // Act.
        var sut = new PluginLoader(options, Substitute.For<ILogger<PluginLoader>>());

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(sut.Plugins()).HasCount().EqualToOne();
        await Assert.That(sut.Plugins()[0].Id).IsEqualTo(new ExportedMetadata().Id);
    }
}
