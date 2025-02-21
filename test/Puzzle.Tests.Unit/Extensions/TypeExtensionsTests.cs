using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;
using Puzzle.Extensions;
using Puzzle.Tests.Unit.TestPlugin.Abstractions;

namespace Puzzle.Tests.Unit.Extensions;

public sealed class TypeExtensionsTests
{
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

[Service<IExclusiveService>(ServiceLifetime.Singleton)]
[ServiceKey<string>(Key)]
file sealed class KeyedService : IExclusiveService
{
    public const string Key = nameof(KeyedService);
}

[Service<IExclusiveService>(ServiceLifetime.Singleton)]
file sealed class UnkeyedService : IExclusiveService;
