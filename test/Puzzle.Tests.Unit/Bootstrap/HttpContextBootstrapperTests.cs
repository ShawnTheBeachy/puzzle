using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.Bootstrap;
using Puzzle.Http;

namespace Puzzle.Tests.Unit.Bootstrap;

public sealed class HttpContextBootstrapperTests
{
    [Test]
    public async Task Bootstrap_ShouldAssignConstructedServicesToHttpContextRequestServices()
    {
        // Arrange.
        var sut = new HttpContextBootstrapper();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(Substitute.For<HttpContext>());
        var baseServices = Substitute.For<IServiceProvider>();
        baseServices.GetService(typeof(IHttpContextAccessor)).Returns(httpContextAccessor);

        // Act.
        var services = new ServiceCollection();
        IServiceProvider? serviceProvider = null;
        var bootstrapped = sut.Bootstrap(
            services,
            baseServices,
            (sc, _) => serviceProvider = sc.BuildServiceProvider()
        );

        // Assert.
        await Assert
            .That(
                bootstrapped.GetRequiredService<IHttpContextAccessor>().HttpContext?.RequestServices
            )
            .IsEqualTo(serviceProvider);
    }

    [Test]
    public async Task Bootstrap_ShouldDoNothing_WhenBaseServiceProviderDoesNotHaveHttpContextAccessor()
    {
        // Arrange.
        var sut = new HttpContextBootstrapper();
        var baseServices = Substitute.For<IServiceProvider>();
        var services = new ServiceCollection();

        // Act.
        var bootstrapped = sut.Bootstrap(
            services,
            baseServices,
            (sc, _) => sc.BuildServiceProvider()
        );

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(bootstrapped.GetService<IHttpContextAccessor>()).IsNull();
        await Assert.That(services).IsEmpty();
    }

    [Test]
    public async Task Bootstrap_ShouldRegisterHttpContextAccessorWithNullHttpContext_WhenBaseServiceProviderHttpContextAccessorHasNullHttpContext()
    {
        // Arrange.
        var sut = new HttpContextBootstrapper();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns((HttpContext?)null);
        var baseServices = Substitute.For<IServiceProvider>();
        baseServices.GetService(typeof(IHttpContextAccessor)).Returns(httpContextAccessor);
        var services = new ServiceCollection();

        // Act.
        var bootstrapped = sut.Bootstrap(
            services,
            baseServices,
            (sc, _) => sc.BuildServiceProvider()
        );

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(services).HasCount().EqualToOne();
        await Assert.That(bootstrapped.GetService<IHttpContextAccessor>()).IsNotNull();
        await Assert
            .That(bootstrapped.GetRequiredService<IHttpContextAccessor>().HttpContext)
            .IsNull();
    }

    [Test]
    public async Task Bootstrap_ShouldRegisterHttpContextAccessorWithPuzzleHttpContext_WhenBaseServiceProviderHasHttpContextAccessor()
    {
        // Arrange.
        var sut = new HttpContextBootstrapper();
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(Substitute.For<HttpContext>());
        var baseServices = Substitute.For<IServiceProvider>();
        baseServices.GetService(typeof(IHttpContextAccessor)).Returns(httpContextAccessor);
        var services = new ServiceCollection();

        // Act.
        var bootstrapped = sut.Bootstrap(
            services,
            baseServices,
            (sc, _) => sc.BuildServiceProvider()
        );

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(services).HasCount().EqualToOne();
        await Assert.That(bootstrapped.GetService<IHttpContextAccessor>()).IsNotNull();
        await Assert
            .That(bootstrapped.GetRequiredService<IHttpContextAccessor>().HttpContext)
            .IsTypeOf<PuzzleHttpContext>();
    }
}
