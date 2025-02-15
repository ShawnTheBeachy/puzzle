using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Puzzle.Options;
using Puzzle.Tests.Unit.TestPluginA;

namespace Puzzle.Tests.Unit;

public sealed class PluginLoaderTests
{
    [Test]
    public async Task Information_ShouldBeLogged_WhenPluginIsDiscovered()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { $"{nameof(PuzzleOptions.Locations)}:0", GlobalHooks.PluginsPath },
                }
            )
            .Build();
        var logger = new TestableLogger<PluginLoader>();

        // Act.
        var sut = new PluginLoader(configuration, logger);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(logger.LogMessages).HasCount().EqualToOne();
        await Assert.That(logger.LogMessages[0].LogLevel).IsEqualTo(LogLevel.Information);
        await Assert
            .That(logger.LogMessages[0].Message)
            .IsEqualTo(
                Logging
                    .Messages.DiscoveredPlugin.Replace("{Plugin}", new ExportedMetadata().Name)
                    .Replace("{PluginId}", new ExportedMetadata().Id)
            );
    }

    [Test]
    public async Task Plugin_ShouldBeLoaded_WhenItExistsInLocationSpecifiedInOptions()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { $"{nameof(PuzzleOptions.Locations)}:0", GlobalHooks.PluginsPath },
                }
            )
            .Build();

        // Act.
        var sut = new PluginLoader(configuration, Substitute.For<ILogger<PluginLoader>>());

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(sut.Plugins()).HasCount().EqualToOne();
        await Assert.That(sut.Plugins()[0].Id).IsEqualTo(new ExportedMetadata().Id);
    }
}
