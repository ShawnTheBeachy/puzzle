using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Puzzle.Options;
using Puzzle.Tests.Unit.TestPlugin;
using Puzzle.Tests.Unit.TestPlugin.Abstractions;

namespace Puzzle.Tests.Unit;

public sealed class PluginLoaderTests
{
    [Test]
    public async Task Assembly_ShouldBeSkipped_WhenItIsNotAPlugin()
    {
        // Arrange.
        var assembly = typeof(PluginLoaderTests).Assembly;
        var fileInfo = new FileInfo(assembly.Location);
        var pluginsDir = fileInfo.Directory?.Parent;

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { $"{nameof(PuzzleOptions.Locations)}:0", pluginsDir?.FullName },
                }
            )
            .Build();

        // Act.
        var loader = new PluginLoader(
            configuration,
            Substitute.For<ILogger<PluginLoader>>(),
            Substitute.For<IServiceProvider>()
        );

        // Assert.
        await Assert.That(loader.Plugins).IsEmpty();
    }

    [Test]
    public async Task GetService_ShouldRespectPluginPriority_WhenMultipleServicesAreFound() { }

    [Test]
    public async Task GetService_ShouldReturnNull_WhenNoServicesAreFound()
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
        var loader = new PluginLoader(
            configuration,
            Substitute.For<ILogger<PluginLoader>>(),
            Substitute.For<IServiceProvider>()
        );

        // Act.
        var resolved = loader.GetService<string>();

        // Assert.
        await Assert.That(resolved).IsNull();
    }

    [Test]
    public async Task GetService_ShouldReturnNull_WhenServiceIsFoundFromDifferentPluginThanSpecified()
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
        var loader = new PluginLoader(
            configuration,
            Substitute.For<ILogger<PluginLoader>>(),
            Substitute.For<IServiceProvider>()
        );

        // Act.
        var resolved = loader.GetService<IService>("fake.plugin");

        // Assert.
        await Assert.That(resolved).IsNull();
    }

    [Test]
    public async Task GetService_ShouldReturnService_WhenServiceIsFound()
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
        var loader = new PluginLoader(
            configuration,
            Substitute.For<ILogger<PluginLoader>>(),
            Substitute.For<IServiceProvider>()
        );

        // Act.
        var resolved = loader.GetService<IService>();

        // Assert.
        await Assert.That(resolved?.GetType().FullName).IsEqualTo(typeof(ExportedService).FullName);
    }

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
        var sut = new PluginLoader(configuration, logger, Substitute.For<IServiceProvider>());

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
        var sut = new PluginLoader(
            configuration,
            Substitute.For<ILogger<PluginLoader>>(),
            Substitute.For<IServiceProvider>()
        );

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(sut.Plugins()).HasCount().EqualToOne();
        await Assert.That(sut.Plugins()[0].Id).IsEqualTo(new ExportedMetadata().Id);
    }
}
