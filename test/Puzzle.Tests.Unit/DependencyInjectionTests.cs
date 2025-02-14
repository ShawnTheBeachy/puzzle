using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Puzzle.Abstractions;
using Puzzle.Tests.Unit.TestPlugin;
using TUnit.Assertions.AssertConditions.Throws;

namespace Puzzle.Tests.Unit;

public sealed class DependencyInjectionTests
{
    [Test]
    public async Task AddPluginsToHostBuild_ShouldPassHostServicesAndConfiguration()
    {
        // Arrange.
        var services = new ServiceCollection();
        var config = Substitute.For<IConfigurationManager>();
        var host = Substitute.For<IHostApplicationBuilder>();
        host.Configuration.Returns(config);
        host.Services.Returns(services);

        // Act.
        IServiceCollection? passedServices = null;
        IConfiguration? passedConfig = null;
        host.AddPlugins(x =>
        {
            passedConfig = x.Configuration;
            passedServices = x.Services;
        });

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(passedConfig).IsSameReferenceAs(config);
        await Assert.That(passedServices).IsSameReferenceAs(services);
    }

    [Test]
    public async Task HostedService_ShouldBeRegisteredAsSingleton_WhenServiceAttributeSpecifiesDifferentLifetime()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PluginOptions.SectionName}:{nameof(PluginOptions.Locations)}:0",
                        GlobalHooks.PluginsPath
                    },
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act.
        services.AddPlugins(configuration);

        // Assert.
        var provider = services.BuildServiceProvider();
        using var asserts = Assert.Multiple();
        await Assert
            .That(provider.GetService<IHostedService>()?.GetType().FullName)
            .IsEqualTo(typeof(ExportedHostedService).FullName);
        await Assert
            .That(services)
            .Contains(x =>
                x.ServiceType == typeof(IHostedService) && x.Lifetime == ServiceLifetime.Singleton
            );
    }

    [Test]
    public async Task PluginLoader_ShouldBeRegisteredAsSingleton()
    {
        // Arrange.
        var services = new ServiceCollection();
        var config = Substitute.For<IConfiguration>();

        // Act.
        var provider = services.AddPlugins(config).BuildServiceProvider();

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert
            .That(services)
            .Contains(x =>
                x.ServiceType == typeof(IPluginLoader)
                && x
                    is { ImplementationInstance: PluginLoader, Lifetime: ServiceLifetime.Singleton }
            );
        await Assert.That(provider.GetService<IPluginLoader>()).IsTypeOf<PluginLoader>();
    }

    [Test]
    public async Task PluginOptions_ShouldBeRegistered()
    {
        // Arrange.
        const string locationA = "/usr/plugins";
        const string locationB = "/var/plugins";
        var threshold = TimeSpan.FromSeconds(5);

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PluginOptions.SectionName}:{nameof(PluginOptions.Locations)}:0",
                        locationA
                    },
                    {
                        $"{PluginOptions.SectionName}:{nameof(PluginOptions.Locations)}:1",
                        locationB
                    },
                    {
                        $"{PluginOptions.SectionName}:{nameof(PluginOptions.StartupThreshold)}",
                        threshold.ToString()
                    },
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act.
        services.AddPlugins(configuration);

        // Assert.
        var provider = services.BuildServiceProvider();
        var resolved = provider.GetService<IOptions<PluginOptions>>();
        using var asserts = Assert.Multiple();
        await Assert.That(resolved).IsNotNull();
        await Assert.That(resolved!.Value.Locations[0]).IsEqualTo(locationA);
        await Assert.That(resolved!.Value.Locations[1]).IsEqualTo(locationB);
        await Assert.That(resolved!.Value.StartupThreshold).IsEqualTo(threshold);
    }

    [Test]
    public async Task PluginService_ShouldBeRegistered_WhenItIsExportedFromPluginInLocationSpecifiedInOptionsAndHasServiceAttribute()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PluginOptions.SectionName}:{nameof(PluginOptions.Locations)}:0",
                        GlobalHooks.PluginsPath
                    },
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act.
        services.AddPlugins(configuration);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert
            .That(services)
            .Contains(x =>
                x.ServiceType == typeof(ITuple) && x.Lifetime == ExportedService.Lifetime
            );
        var provider = services.BuildServiceProvider();
        await Assert
            .That(provider.GetService<ITuple>()?.GetType().FullName)
            .IsEqualTo(typeof(ExportedService).FullName);
    }

    [Test]
    public async Task PluginService_ShouldBeRegisteredInIsolation()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PluginOptions.SectionName}:{nameof(PluginOptions.Locations)}:0",
                        GlobalHooks.PluginsPath
                    },
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act.
        var provider = services.AddPlugins(configuration).BuildServiceProvider();

        // Assert.
        var resolve = () => provider.GetRequiredService<ICloneable>();
        await Assert
            .That(resolve)
            .ThrowsExactly<InvalidOperationException>()
            .WithMessage(
                $"Unable to resolve service for type '{typeof(IFormatProvider)}' while attempting to activate '{typeof(ExportedDependentService)}'."
            );
    }

    [Test]
    public async Task PluginService_ShouldNotBeRegistered_WhenItIsExportedFromPluginInLocationSpecifiedInOptionsButDoesNotHaveServiceAttribute()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PluginOptions.SectionName}:{nameof(PluginOptions.Locations)}:0",
                        GlobalHooks.PluginsPath
                    },
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act.
        services.AddPlugins(configuration);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(services).DoesNotContain(x => x.ServiceType == typeof(IFormatProvider));
        var provider = services.BuildServiceProvider();
        await Assert.That(provider.GetService<IFormatProvider>()).IsNull();
    }

    [Test]
    public async Task Warning_ShouldBeLogged_WhenStartupTakesLongerThanSpecifiedThreshold()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { { "Plugins:StartupThreshold", "00:00:00.00000" } }
            )
            .Build();
        var logger = new TestableLogger<PluginLoader>();
        var services = new ServiceCollection();
        services.AddSingleton<ILogger<PluginLoader>>(logger);

        // Act.
        services.AddPlugins(configuration);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(logger.LogMessages).HasCount().EqualToOne();
        await Assert.That(logger.LogMessages[0].LogLevel).IsEqualTo(LogLevel.Warning);
        await Assert
            .That(logger.LogMessages[0].Message)
            .Matches(Logging.Messages.StartupThresholdWarning.Replace("{Elapsed}", "*"));
    }
}
