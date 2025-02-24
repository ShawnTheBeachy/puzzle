using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Puzzle.Abstractions;
using Puzzle.Bootstrap;
using Puzzle.Tests.Unit.TestPlugin;

namespace Puzzle.Tests.Unit.Bootstrap;

public sealed class PluginExtensionsTests
{
    [Test]
    public async Task Bootstrap_ShouldBootstrapHttpContext()
    {
        // Arrange.
        var plugin = new Plugin(
            Substitute.For<ITypeProvider>(),
            typeof(PluginExtensionsTests).Assembly,
            Substitute.For<IPluginMetadata>()
        );
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns((HttpContext?)null);
        var baseServices = Substitute.For<IServiceProvider>();
        baseServices.GetService(typeof(IHttpContextAccessor)).Returns(httpContextAccessor);
        var services = new ServiceCollection();

        // Act.
        var bootstrapped = plugin.Bootstrap(services, baseServices);

        // Assert.
        await Assert.That(bootstrapped.GetService<IHttpContextAccessor>()).IsNotNull();
    }

    [Test]
    public async Task Bootstrap_ShouldBootstrapLogging()
    {
        // Arrange.
        var plugin = new Plugin(
            Substitute.For<ITypeProvider>(),
            typeof(PluginExtensionsTests).Assembly,
            Substitute.For<IPluginMetadata>()
        );
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var baseServices = Substitute.For<IServiceProvider>();
        baseServices.GetService(typeof(ILoggerFactory)).Returns(loggerFactory);
        var services = new ServiceCollection();

        // Act.
        var bootstrapped = plugin.Bootstrap(services, baseServices);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert
            .That(bootstrapped.GetService<ILoggerFactory>())
            .IsSameReferenceAs(loggerFactory);
        await Assert.That(bootstrapped.GetService<ILogger<PluginExtensionsTests>>()).IsNotNull();
    }

    [Test]
    public async Task Bootstrap_ShouldBootstrapPluginBootstrapper()
    {
        // Arrange.
        var plugin = new Plugin(
            Substitute.For<ITypeProvider>(),
            typeof(PluginExtensionsTests).Assembly,
            Substitute.For<IPluginMetadata>(),
            typeof(TestBootstrapper)
        );
        var baseServices = Substitute.For<IServiceProvider>();
        var services = new ServiceCollection();

        // Act.
        var bootstrapped = plugin.Bootstrap(services, baseServices);

        // Assert.
        await Assert.That(bootstrapped.GetService<string>()).IsEqualTo(TestBootstrapper.Foo);
    }

    [Test]
    public async Task Bootstrap_ShouldNotThrow_WhenPluginBootstrapperTypeIsNull()
    {
        // Arrange.
        var plugin = new Plugin(
            Substitute.For<ITypeProvider>(),
            typeof(PluginExtensionsTests).Assembly,
            new ExportedMetadata()
        );
        var services = new ServiceCollection();

        // Act.
        var bootstrapped = plugin.Bootstrap(services, Substitute.For<IConfiguration>());

        // Assert.
        await Assert.That(bootstrapped).IsSameReferenceAs(services);
    }
}

file sealed class TestBootstrapper : IPluginBootstrapper
{
    public const string Foo = "foo";

    public IServiceCollection Bootstrap(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<string>(Foo);
        return services;
    }
}
