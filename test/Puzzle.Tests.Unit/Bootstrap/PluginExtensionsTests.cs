using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Puzzle.Abstractions;
using Puzzle.Bootstrap;

namespace Puzzle.Tests.Unit.Bootstrap;

public sealed class PluginExtensionsTests
{
    [Test]
    public async Task Bootstrap_ShouldBootstrapHttpContext()
    {
        // Arrange.
        var plugin = new Plugin(Substitute.For<ITypeProvider>(), null!);
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
        var plugin = new Plugin(Substitute.For<ITypeProvider>(), null!);
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var baseServices = Substitute.For<IServiceProvider>();
        baseServices.GetService(typeof(ILoggerFactory)).Returns(loggerFactory);
        var services = new ServiceCollection();

        // Act.
        var bootstrapped = plugin.Bootstrap(services, baseServices);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(bootstrapped.GetService<ILoggerFactory>()).IsEqualTo(loggerFactory);
        await Assert.That(bootstrapped.GetService<ILogger<PluginExtensionsTests>>()).IsNotNull();
    }
}
