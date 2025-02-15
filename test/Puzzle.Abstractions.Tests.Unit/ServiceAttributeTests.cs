using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Puzzle.Abstractions.Tests.Unit;

public sealed class ServiceAttributeTests
{
    [Test]
    public async Task GenericAttribute_ShouldBeAssignableToClasses()
    {
        // Arrange.
        var usage = typeof(ServiceAttribute<>).GetCustomAttribute<AttributeUsageAttribute>()!;

        // Assert.
        await Assert.That(usage.ValidOn).IsEqualTo(AttributeTargets.Class);
    }

    [Test]
    public async Task GenericAttribute_ShouldInheritFromNonGenericAttribute()
    {
        await Assert
            .That(new ServiceAttribute<string>(ServiceLifetime.Scoped))
            .IsAssignableTo<ServiceAttribute>();
    }

    [Test]
    public async Task NonGenericAttribute_ShouldBeAssignableToClasses()
    {
        // Arrange.
        var usage = typeof(ServiceAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;

        // Assert.
        await Assert.That(usage.ValidOn).IsEqualTo(AttributeTargets.Class);
    }

    [Test]
    public async Task NonGenericAttribute_ShouldInheritFromAttribute()
    {
        await Assert.That(new ServiceAttribute(ServiceLifetime.Scoped)).IsAssignableTo<Attribute>();
    }
}
