using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;

namespace Puzzle.Blazor.Tests.Unit;

public sealed class DependencyInjectionTests
{
    [Test]
    public async Task AddBlazor_ShouldAddComponentsFromPlugin_WhenPluginHasComponents()
    {
        // Arrange.
        var typeProviderA = Substitute.For<ITypeProvider>();
        typeProviderA.GetTypes().Returns([typeof(ComponentA), typeof(string)]);
        var pluginA = new Plugin(typeProviderA);

        var typeProviderB = Substitute.For<ITypeProvider>();
        typeProviderB.GetTypes().Returns([typeof(ComponentB), typeof(int)]);
        var pluginB = new Plugin(typeProviderB);

        var services = new ServiceCollection();
        var config = new PuzzleConfiguration(
            [pluginA, pluginB],
            Substitute.For<IConfiguration>(),
            services
        );

        // Act.
        config.AddBlazor();

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(services).HasCount().EqualTo(2);

        await Assert.That(services[0].ServiceType).IsEqualTo(typeof(ComponentA));
        await Assert.That(services[0].Lifetime).IsEqualTo(ServiceLifetime.Transient);
        await Assert.That(services[0].ImplementationFactory).IsNotNull();
        var resolvedA = services[0].ImplementationFactory!(Substitute.For<IServiceProvider>());
        await Assert.That(resolvedA).IsTypeOf<ComponentA>();

        await Assert.That(services[1].ImplementationFactory).IsNotNull();
        await Assert.That(services[1].ServiceType).IsEqualTo(typeof(ComponentB));
        await Assert.That(services[1].Lifetime).IsEqualTo(ServiceLifetime.Transient);
        var resolvedB = services[0].ImplementationFactory!(Substitute.For<IServiceProvider>());
        await Assert.That(resolvedB).IsTypeOf<ComponentA>();
    }
}

file sealed class ComponentA : TestComponentBase;

file sealed class ComponentB : TestComponentBase;
