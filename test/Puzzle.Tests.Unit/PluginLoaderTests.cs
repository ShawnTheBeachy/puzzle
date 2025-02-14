﻿using Microsoft.Extensions.Logging;
using Puzzle.Tests.Unit.TestPlugin;

namespace Puzzle.Tests.Unit;

public sealed class PluginLoaderTests
{
    [Test]
    public async Task Information_ShouldBeLogged_WhenPluginIsDiscovered()
    {
        // Arrange.
        var options = new PuzzleOptions { Locations = [GlobalHooks.PluginsPath] };
        var logger = new TestableLogger<PluginLoader>();

        // Act.
        var sut = new PluginLoader(options, logger);

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
        var options = new PuzzleOptions { Locations = [GlobalHooks.PluginsPath] };

        // Act.
        var sut = new PluginLoader(options, Substitute.For<ILogger<PluginLoader>>());

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(sut.Plugins()).HasCount().EqualToOne();
        await Assert.That(sut.Plugins()[0].Id).IsEqualTo(new ExportedMetadata().Id);
    }
}
