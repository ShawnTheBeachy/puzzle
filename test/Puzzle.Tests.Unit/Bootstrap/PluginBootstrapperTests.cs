using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.EnvironmentVariables;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Puzzle.Abstractions;
using Puzzle.Bootstrap;

namespace Puzzle.Tests.Unit.Bootstrap;

public sealed class PluginBootstrapperTests
{
    [Test]
    public async Task Bootstrap_ShouldDoNothing_WhenPluginBootstrapperTypeIsNull()
    {
        // Arrange.
        var plugin = new Plugin(
            Substitute.For<ITypeProvider>(),
            typeof(PluginBootstrapperTests).Assembly,
            Substitute.For<IPluginMetadata>()
        );
        var sut = new PluginBootstrapper(plugin);

        var services = new ServiceCollection();
        var baseProvider = Substitute.For<IServiceProvider>();

        // Act.
        _ = sut.Bootstrap(services, baseProvider, (sc, _) => sc.BuildServiceProvider());

        // Assert.
        await Assert.That(services).IsEmpty();
    }

    [Test]
    public async Task Bootstrap_ShouldExecutePluginBootstrapper_WhenPluginBootstrapperTypeIsNotNull()
    {
        // Arrange.
        var plugin = new Plugin(
            Substitute.For<ITypeProvider>(),
            typeof(PluginBootstrapperTests).Assembly,
            Substitute.For<IPluginMetadata>(),
            typeof(TestBootstrapper)
        );
        var sut = new PluginBootstrapper(plugin);

        var services = new ServiceCollection();
        var baseProvider = Substitute.For<IServiceProvider>();

        // Act.
        var bootstrapped = sut.Bootstrap(
            services,
            baseProvider,
            (sc, _) => sc.BuildServiceProvider()
        );

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert
            .That(bootstrapped.GetRequiredService<string>())
            .IsEqualTo(TestBootstrapper.Foo);
    }

    [Test]
    public async Task Bootstrap_ShouldExecutePluginBootstrapperWithConfiguration_WhenPluginBootstrapperTypeIsNotNull()
    {
        // Arrange.
        var plugin = new Plugin(
            Substitute.For<ITypeProvider>(),
            typeof(PluginBootstrapperTests).Assembly,
            Substitute.For<IPluginMetadata>(),
            typeof(TestBootstrapper)
        );
        var sut = new PluginBootstrapper(plugin);

        var services = new ServiceCollection();
        var baseProvider = Substitute.For<IServiceProvider>();

        // Act.
        var bootstrapped = sut.Bootstrap(
            services,
            baseProvider,
            (sc, _) => sc.BuildServiceProvider()
        );

        // Assert.
        using var asserts = Assert.Multiple();
        var configProviders = bootstrapped.GetServices<IConfigurationProvider>().ToArray();
        await Assert.That(configProviders).HasCount().EqualTo(2);
        await Assert.That(configProviders).Contains(x => x is JsonConfigurationProvider);
        var jsonProvider = configProviders.OfType<JsonConfigurationProvider>().ToArray()[0];
        await Assert.That(jsonProvider.Source.Path).IsEqualTo("settings.json");
        await Assert.That(jsonProvider.Source.Optional).IsTrue();
        await Assert.That(jsonProvider.Source.ReloadOnChange).IsTrue();
        await Assert.That(jsonProvider.Source.FileProvider).IsTypeOf<PhysicalFileProvider>();
        await Assert
            .That(
                ((PhysicalFileProvider)jsonProvider.Source.FileProvider!).Root.TrimEnd(
                    Path.DirectorySeparatorChar
                )
            )
            .IsEqualTo(
                new FileInfo(typeof(PluginBootstrapperTests).Assembly.Location).DirectoryName
            );
        await Assert
            .That(configProviders)
            .Contains(x => x is EnvironmentVariablesConfigurationProvider);
    }
}

file sealed class TestBootstrapper : IPluginBootstrapper
{
    public const string Foo = "foo";

    public IServiceCollection Bootstrap(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<string>(Foo);
        services.TryAddEnumerable(
            ((ConfigurationRoot)configuration).Providers.Select(ServiceDescriptor.Singleton)
        );
        return services;
    }
}
