using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Puzzle.Abstractions;
using Puzzle.Options;
using Puzzle.Tests.Unit.TestPlugin;
using Puzzle.Tests.Unit.TestPlugin.Abstractions;
using TUnit.Assertions.AssertConditions.Throws;

namespace Puzzle.Tests.Unit;

public sealed class DependencyInjectionTests
{
    [Test]
    public async Task AddPlugins_ShouldNotThrow_WhenPluginSectionIsNotInConfiguration()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        // Act.
        var addPlugins = () => services.AddPlugins(configuration);

        // Assert.
        await Assert.That(addPlugins).ThrowsNothing();
    }

    [Test]
    public async Task AddPluginsToHostBuild_ShouldPassHostServices()
    {
        // Arrange.
        var services = new ServiceCollection();
        var host = Substitute.For<IHostApplicationBuilder>();
        host.Configuration.Returns(Substitute.For<IConfigurationManager>());
        host.Services.Returns(services);

        // Act.
        IServiceCollection? passedServices = null;
        host.AddPlugins(x =>
        {
            passedServices = x.Services;
        });

        // Assert.
        await Assert.That(passedServices).IsSameReferenceAs(services);
    }

    [Test]
    public async Task ExclusiveService_ShouldBeReplaced_WhenItIsExportedFromPlugin()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.Locations)}:0",
                        GlobalHooks.PluginsPath
                    },
                }
            )
            .Build();
        var services = new ServiceCollection().AddSingleton(Substitute.For<IExclusiveService>());

        // Act.
        var provider = services.AddPlugins(configuration).BuildServiceProvider();

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert
            .That(services.Where(x => x.ServiceType == typeof(IExclusiveService)))
            .HasCount()
            .EqualToOne();
        await Assert
            .That(services)
            .Contains(x =>
                x.Lifetime == ExportedExclusiveService.Lifetime
                && x.ImplementationFactory is not null
            );
        await Assert
            .That(provider.GetService<IExclusiveService>()?.GetType().FullName)
            .IsEqualTo(typeof(ExportedExclusiveService).FullName);
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
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.Locations)}:0",
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
    public async Task PluginBootstrapper_ShouldBeExecutedOnPluginsOwnOptionsConfiguration_WhenIsolatePluginsIsSetToFalseInConfiguration()
    {
        // Arrange.
        const string expectedValue = "Bar";
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.Locations)}:0",
                        GlobalHooks.PluginsPath
                    },
                    {
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.IsolatePlugins)}",
                        "false"
                    },
                    {
                        $"{PuzzleOptions.SectionName}:{new ExportedMetadata().Id}:Options:Foo",
                        expectedValue
                    },
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act.
        var provider = services.AddPlugins(configuration).BuildServiceProvider();

        // Assert.
        var bootstrapper = (ExportedBootstrapper)provider.GetRequiredService<IPluginBootstrapper>();
        await Assert
            .That(bootstrapper.Configuration?.GetValue<string>("Foo"))
            .IsEqualTo(expectedValue);
    }

    [Test]
    public async Task PluginBootstrapper_ShouldBeExecutedOnServiceCollection_WhenIsolatePluginsIsSetToFalseInConfiguration()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.Locations)}:0",
                        GlobalHooks.PluginsPath
                    },
                    {
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.IsolatePlugins)}",
                        "false"
                    },
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act.
        var provider = services.AddPlugins(configuration).BuildServiceProvider();

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(services).Contains(x => x.ServiceType == typeof(IPluginBootstrapper));
        var service = services.First(x => x.ServiceType == typeof(IPluginBootstrapper));
        await Assert.That(service.ImplementationInstance).IsTypeOf<ExportedBootstrapper>();
        var bootstrapper = provider.GetService<IPluginBootstrapper>();
        await Assert.That(bootstrapper).IsTypeOf<ExportedBootstrapper>();
        await Assert
            .That(((ExportedBootstrapper)bootstrapper!).Services)
            .IsSameReferenceAs(services);
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
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.Locations)}:0",
                        locationA
                    },
                    {
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.Locations)}:1",
                        locationB
                    },
                    {
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.StartupThreshold)}",
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
        var resolved = provider.GetService<IOptions<PuzzleOptions>>();
        using var asserts = Assert.Multiple();
        await Assert.That(resolved).IsNotNull();
        await Assert.That(resolved!.Value.Locations[0]).IsEqualTo(locationA);
        await Assert.That(resolved.Value.Locations[1]).IsEqualTo(locationB);
        await Assert.That(resolved.Value.StartupThreshold).IsEqualTo(threshold);
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
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.Locations)}:0",
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
                x.ServiceType == typeof(IService) && x.Lifetime == ExportedService.Lifetime
            );
        var provider = services.BuildServiceProvider();
        await Assert
            .That(provider.GetService<IService>()?.GetType().FullName)
            .IsEqualTo(typeof(ExportedService).FullName);
    }

    [Test]
    public async Task PluginService_ShouldBeRegisteredInIsolation_WhenIsolatedIsNotOverridenInConfiguration()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.Locations)}:0",
                        GlobalHooks.PluginsPath
                    },
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act.
        var provider = services.AddPlugins(configuration).BuildServiceProvider();

        // Assert.
        var resolve = () => provider.GetRequiredService<IDependentService>();
        await Assert
            .That(resolve)
            .ThrowsExactly<InvalidOperationException>()
            .WithMessage(
                $"Unable to resolve service for type '{typeof(IDependentService.IDependency)}' while attempting to activate '{typeof(ExportedDependentService)}'."
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
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.Locations)}:0",
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
            .DoesNotContain(x => x.ServiceType == typeof(IServiceWithoutAttribute));
        var provider = services.BuildServiceProvider();
        await Assert.That(provider.GetService<IServiceWithoutAttribute>()).IsNull();
    }

    [Test]
    public async Task PluginService_ShouldNotBeRegistered_WhenItIsExportedFromPluginWhichIsDisabledInConfiguration()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.Locations)}:0",
                        GlobalHooks.PluginsPath
                    },
                    { $"{PuzzleOptions.SectionName}:{new ExportedMetadata().Id}:Disabled", "true" },
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act.
        services.AddPlugins(configuration);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(services).DoesNotContain(x => x.ServiceType == typeof(IService));
        var provider = services.BuildServiceProvider();
        await Assert.That(provider.GetService<IService>()).IsNull();
    }

    [Test]
    public async Task PluginService_ShouldNotBeRegisteredInIsolation_WhenIsolatePluginsIsSetToFalseInConfiguration()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PuzzleOptions.SectionName}:{nameof(PuzzleOptions.Locations)}:0",
                        GlobalHooks.PluginsPath
                    },
                    { $"{PuzzleOptions.SectionName}:IsolatePlugins", "false" },
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act.
        var provider = services
            .AddSingleton(Substitute.For<IDependentService.IDependency>())
            .AddPlugins(configuration)
            .BuildServiceProvider();

        // Assert.
        using var asserts = Assert.Multiple();
        var service = services.First(x => x.ServiceType == typeof(IDependentService));
        await Assert.That(service.ImplementationType).IsEqualTo(typeof(ExportedDependentService));
        await Assert
            .That(provider.GetService<IDependentService>())
            .IsTypeOf<ExportedDependentService>();
    }

    [Test]
    public async Task PluginService_ShouldReplaceOtherPluginService_WhenItsPriorityIsHigher()
    {
        // Arrange.
        var typesA = Substitute.For<ITypeProvider>();
        typesA.GetTypes().Returns([typeof(ServiceA)]);

        var typesB = Substitute.For<ITypeProvider>();
        typesB.GetTypes().Returns([typeof(ServiceB)]);

        var metadata = Substitute.For<IPluginMetadata>();

        var pluginA = new Plugin(
            typesA,
            typeof(DependencyInjectionTests).Assembly,
            metadata,
            priority: 1
        );
        var pluginB = new Plugin(
            typesB,
            typeof(DependencyInjectionTests).Assembly,
            metadata,
            priority: 2
        );

        var services = new ServiceCollection();

        // Act.
        var provider = services
            .AddPlugins([pluginA, pluginB], Substitute.For<IConfiguration>())
            .BuildServiceProvider();

        // Assert.
        await Assert.That(provider.GetService<IExclusiveService>()).IsTypeOf<ServiceA>();
    }

    [Test]
    public async Task PluginService_ShouldReplaceOtherPluginService_WhenOtherPriorityIsNull()
    {
        // Arrange.
        var typesA = Substitute.For<ITypeProvider>();
        typesA.GetTypes().Returns([typeof(ServiceA)]);

        var typesB = Substitute.For<ITypeProvider>();
        typesB.GetTypes().Returns([typeof(ServiceB)]);

        var metadata = Substitute.For<IPluginMetadata>();

        var pluginA = new Plugin(
            typesA,
            typeof(DependencyInjectionTests).Assembly,
            metadata,
            priority: 1
        );
        var pluginB = new Plugin(typesB, typeof(DependencyInjectionTests).Assembly, metadata);

        var services = new ServiceCollection();

        // Act.
        var provider = services
            .AddPlugins([pluginA, pluginB], Substitute.For<IConfiguration>())
            .BuildServiceProvider();

        // Assert.
        await Assert.That(provider.GetService<IExclusiveService>()).IsTypeOf<ServiceA>();
    }

    [Test]
    public async Task PuzzleConfigurationSection_ShouldBePassedToConfigAction()
    {
        // Arrange.
        var config = Substitute.For<IConfigurationManager>();
        var puzzleConfig = Substitute.For<IConfigurationSection>();
        config.GetSection(PuzzleOptions.SectionName).Returns(puzzleConfig);

        var host = Substitute.For<IHostApplicationBuilder>();
        host.Configuration.Returns(config);
        host.Services.Returns(new ServiceCollection());

        // Act.
        IConfiguration? passedConfig = null;
        host.AddPlugins(x =>
        {
            passedConfig = x.Configuration;
        });

        // Assert.
        await Assert.That(passedConfig).IsSameReferenceAs(puzzleConfig);
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

[Service<IExclusiveService>(ServiceLifetime.Scoped)]
file sealed class ServiceA : IExclusiveService;

[Service<IExclusiveService>(ServiceLifetime.Scoped)]
file sealed class ServiceB : IExclusiveService;
