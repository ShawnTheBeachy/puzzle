using Microsoft.Extensions.DependencyInjection;

namespace Puzzle.Blazor.Tests.Unit;

public sealed class ServiceProviderComponentActivatorTests
{
    [Test]
    public async Task CreateInstance_ShouldResolveComponentFromDI_WhenItIsInServiceProvider()
    {
        // Arrange.
        var service = new Service();
        var serviceProvider = new ServiceCollection()
            .AddTransient<ResolvableComponent>()
            .AddSingleton(service)
            .BuildServiceProvider();
        var sut = new ServiceProviderComponentActivator(serviceProvider);

        // Act.
        var instance = sut.CreateInstance(typeof(ResolvableComponent));

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(instance).IsNotNull();
        await Assert.That(instance).IsTypeOf<ResolvableComponent>();
        await Assert.That(((ResolvableComponent)instance).Service).IsEqualTo(service);
    }

    [Test]
    public async Task CreateInstance_ShouldUseActivator_WhenComponentIsNotInServiceProvider()
    {
        // Arrange.
        var serviceProvider = new ServiceCollection().BuildServiceProvider();
        var sut = new ServiceProviderComponentActivator(serviceProvider);

        // Act.
        var instance = sut.CreateInstance(typeof(NonResolvableComponent));

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(instance).IsNotNull();
        await Assert.That(instance).IsTypeOf<NonResolvableComponent>();
    }
}

file sealed class Service;

file sealed class NonResolvableComponent : TestComponentBase;

file sealed class ResolvableComponent(Service service) : TestComponentBase
{
    public Service Service => service;
}
