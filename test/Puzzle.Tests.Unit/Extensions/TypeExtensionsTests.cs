using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;
using Puzzle.Extensions;
using Puzzle.Tests.Unit.TestPlugin;
using Puzzle.Tests.Unit.TestPlugin.Abstractions;
using TUnit.Assertions.AssertConditions.Throws;

namespace Puzzle.Tests.Unit.Extensions;

public sealed class TypeExtensionsTests
{
    [Test]
    public async Task GetService_ShouldThrow_WhenPluginServiceIsRegisteredForAbstractionWhichItDoesNotImplement()
    {
        // Arrange.
        var typeProvider = Substitute.For<ITypeProvider>();
        typeProvider.GetTypes().Returns([typeof(InvalidService)]);

        var plugin = new Plugin(
            typeProvider,
            typeof(DependencyInjectionTests).Assembly,
            new ExportedMetadata()
        );
        var services = new ServiceCollection();

        // Assert.
        await Assert
            .That(Act)
            .ThrowsExactly<Exception>()
            .WithMessage(
                TypeExtensions.Messages.ServiceNotImplemented(
                    typeof(InvalidService),
                    typeof(IService)
                )
            );
        return;

        // Act.
        void Act() => services.AddPlugins([plugin], Substitute.For<IConfiguration>());
    }

    [Test]
    public async Task GetServiceKey_ShouldReturnKey_WhenServiceKeyAttributeIsPresent()
    {
        // Arrange.
        var type = typeof(KeyedService);

        // Act.
        var key = type.GetServiceKey();

        // Assert.
        await Assert.That(key).IsEqualTo(KeyedService.Key);
    }

    [Test]
    public async Task GetServiceKey_ShouldReturnNull_WhenServiceKeyAttributeIsNotPresent()
    {
        // Arrange.
        var type = typeof(UnkeyedService);

        // Act.
        var key = type.GetServiceKey();

        // Assert.
        await Assert.That(key).IsNull();
    }
}

[Service<IService>(Lifetime)]
file sealed class InvalidService
{
    public const ServiceLifetime Lifetime = ServiceLifetime.Scoped;
}

[Service<IExclusiveService>(ServiceLifetime.Singleton)]
[ServiceKey<string>(Key)]
file sealed class KeyedService : IExclusiveService
{
    public const string Key = nameof(KeyedService);
}

[Service<IExclusiveService>(ServiceLifetime.Singleton)]
file sealed class UnkeyedService : IExclusiveService;
