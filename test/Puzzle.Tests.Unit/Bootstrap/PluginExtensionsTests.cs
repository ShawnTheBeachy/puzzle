using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
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
        baseServices.GetService(Arg.Is(typeof(IHttpContextAccessor))).Returns(httpContextAccessor);
        var services = new ServiceCollection();

        // Act.
        var bootstrapped = plugin.Bootstrap(services, baseServices);

        // Assert.
        await Assert.That(bootstrapped.GetService<IHttpContextAccessor>()).IsNotNull();
    }
}
