using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Puzzle.Bootstrap;

namespace Puzzle.Tests.Unit.Bootstrap;

public sealed class LoggingBootstrapperTests
{
    [Test]
    public async Task Bootstrap_ShouldAddLogging()
    {
        // Arrange.
        var services = new ServiceCollection();
        var sut = new LoggingBootstrapper();

        // Act.
        var bootstrapped = sut.Bootstrap(
            services,
            Substitute.For<IServiceProvider>(),
            (sc, _) => sc.BuildServiceProvider()
        );

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(services).HasCount().EqualTo(8);
        await Assert.That(bootstrapped.GetService<ILogger<LoggingBootstrapperTests>>()).IsNotNull();
    }

    [Test]
    public async Task Bootstrap_ShouldRegisterLoggerFactoryFromBaseServiceProvider_WhenBaseServiceProviderHasLoggingAdded()
    {
        // Arrange.
        var loggerFactory = Substitute.For<ILoggerFactory>();
        var baseProvider = Substitute.For<IServiceProvider>();
        baseProvider.GetService(typeof(ILoggerFactory)).Returns(loggerFactory);
        var services = new ServiceCollection();
        var sut = new LoggingBootstrapper();

        // Act.
        var bootstrapped = sut.Bootstrap(
            services,
            baseProvider,
            (sc, _) => sc.BuildServiceProvider()
        );

        // Assert.
        await Assert.That(bootstrapped.GetService<ILoggerFactory>()).IsEqualTo(loggerFactory);
    }
}
