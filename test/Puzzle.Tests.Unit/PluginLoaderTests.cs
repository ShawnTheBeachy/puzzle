using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Puzzle.Abstractions;
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
        var loader = PluginLoader.Create(
            configuration,
            Substitute.For<ILogger<PluginLoader>>(),
            Substitute.For<IServiceProvider>()
        );

        // Assert.
        await Assert.That(loader.Plugins).IsEmpty();
    }

    [Test]
    public async Task GetService_ShouldRespectPluginPriority_WhenMultipleServicesAreFound()
    {
        // Arrange.
        var typeProviderA = Substitute.For<ITypeProvider>();
        typeProviderA.GetTypes().Returns([typeof(ServiceA)]);

        var typeProviderB = Substitute.For<ITypeProvider>();
        typeProviderB.GetTypes().Returns([typeof(ServiceB)]);

        var pluginA = new Plugin(
            typeProviderA,
            typeof(PluginLoaderTests).Assembly,
            new ExportedMetadata(),
            priority: 2
        );
        var pluginB = new Plugin(
            typeProviderB,
            typeof(PluginLoaderTests).Assembly,
            new ExportedMetadata(),
            priority: 1
        );

        var loader = new PluginLoader([pluginA, pluginB], null, Substitute.For<IServiceProvider>());

        // Act.
        var resolved = loader.GetService<IService>();

        // Assert.
        await Assert.That(resolved).IsTypeOf<ServiceB>();
    }

    [Test]
    public async Task GetService_ShouldReturnNull_WhenNoServicesAreFound()
    {
        // Arrange.
        var plugin = new Plugin(
            Substitute.For<ITypeProvider>(),
            typeof(PluginLoaderTests).Assembly,
            new ExportedMetadata()
        );

        var loader = new PluginLoader([plugin], null, Substitute.For<IServiceProvider>());

        // Act.
        var resolved = loader.GetService<IService>();

        // Assert.
        await Assert.That(resolved).IsNull();
    }

    [Test]
    public async Task GetService_ShouldReturnNull_WhenServiceIsFoundFromDifferentPluginThanSpecified()
    {
        // Arrange.
        var typeProvider = Substitute.For<ITypeProvider>();
        typeProvider.GetTypes().Returns([typeof(ExportedService)]);

        var plugin = new Plugin(
            typeProvider,
            typeof(PluginLoaderTests).Assembly,
            new ExportedMetadata()
        );

        var loader = new PluginLoader([plugin], null, Substitute.For<IServiceProvider>());

        // Act.
        var resolved = loader.GetService<IService>("fake.plugin");

        // Assert.
        await Assert.That(resolved).IsNull();
    }

    [Test]
    public async Task GetService_ShouldReturnService_WhenPluginIdIsNotSpecifiedAndServiceIsFoundFromAnyPlugin()
    {
        // Arrange.
        var typeProvider = Substitute.For<ITypeProvider>();
        typeProvider.GetTypes().Returns([typeof(ExportedService)]);

        var plugin = new Plugin(
            typeProvider,
            typeof(PluginLoaderTests).Assembly,
            new ExportedMetadata()
        );

        var loader = new PluginLoader([plugin], null, Substitute.For<IServiceProvider>());

        // Act.
        var resolved = loader.GetService<IService>();

        // Assert.
        await Assert.That(resolved).IsTypeOf<ExportedService>();
    }

    [Test]
    public async Task GetService_ShouldReturnService_WhenPluginIdIsSpecifiedAndServiceIsFoundFromThatPlugin()
    {
        // Arrange.
        var typeProvider = Substitute.For<ITypeProvider>();
        typeProvider.GetTypes().Returns([typeof(ExportedService)]);

        var plugin = new Plugin(
            typeProvider,
            typeof(PluginLoaderTests).Assembly,
            new ExportedMetadata()
        );

        var loader = new PluginLoader([plugin], null, Substitute.For<IServiceProvider>());

        // Act.
        var resolved = loader.GetService<IService>(new ExportedMetadata().Id);

        // Assert.
        await Assert.That(resolved).IsTypeOf<ExportedService>();
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
        var sut = PluginLoader.Create(configuration, logger, Substitute.For<IServiceProvider>());

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
        var sut = PluginLoader.Create(
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

[Service<IService>]
file sealed class ServiceA : IService;

[Service<IService>]
file sealed class ServiceB : IService;
