using System.Reflection;

namespace Puzzle.Abstractions.Tests.Unit;

public sealed class ExclusiveAttributeTests
{
    [Test]
    public async Task ExclusiveAttribute_ShouldBeAssignableToClassesAndInterfaces()
    {
        // Arrange.
        var usage = typeof(ExclusiveAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;

        // Assert.
        await Assert
            .That(usage.ValidOn)
            .IsEqualTo(AttributeTargets.Class | AttributeTargets.Interface);
    }

    [Test]
    public async Task ExclusiveAttribute_ShouldInheritFromAttribute()
    {
        await Assert.That(new ExclusiveAttribute()).IsAssignableTo<Attribute>();
    }
}
