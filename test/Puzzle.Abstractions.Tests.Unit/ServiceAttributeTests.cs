using Microsoft.Extensions.DependencyInjection;

namespace Puzzle.Abstractions.Tests.Unit;

public sealed class ServiceAttributeTests
{
    [Test]
    public async Task GenericAttribute_ShouldInheritFromNonGenericAttribute()
    {
        await Assert
            .That(new ServiceAttribute<string>(ServiceLifetime.Scoped))
            .IsAssignableTo<ServiceAttribute>();
    }

    [Test]
    public async Task NonGenericAttribute_ShouldInheritFromAttribute()
    {
        await Assert.That(new ServiceAttribute(ServiceLifetime.Scoped)).IsAssignableTo<Attribute>();
    }
}
