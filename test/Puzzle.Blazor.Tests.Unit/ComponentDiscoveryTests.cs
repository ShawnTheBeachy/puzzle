using Microsoft.AspNetCore.Components;

namespace Puzzle.Blazor.Tests.Unit;

public sealed class ComponentDiscoveryTests
{
    [Test]
    public async Task FindComponents_ShouldNotReturnType_WhenItDoesNotImplementIComponent()
    {
        // Arrange.
        IReadOnlyList<Type> types = [typeof(string)];

        // Act.
        var componentTypes = types.FindComponents().ToArray();

        // Assert.
        await Assert.That(componentTypes).IsEmpty();
    }

    [Test]
    public async Task FindComponents_ShouldNotReturnType_WhenItImplementsIComponentButIsAbstract()
    {
        // Arrange.
        IReadOnlyList<Type> types = [typeof(TestComponentBase)];

        // Act.
        var componentTypes = types.FindComponents().ToArray();

        // Assert.
        await Assert.That(componentTypes).IsEmpty();
    }

    [Test]
    public async Task FindComponents_ShouldReturnType_WhenItImplementsIComponent()
    {
        // Arrange.
        IReadOnlyList<Type> types = [typeof(ComponentType)];

        // Act.
        var componentTypes = types.FindComponents().ToArray();

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(componentTypes).HasCount().EqualToOne();
        await Assert.That(componentTypes[0]).IsSameReferenceAs(types[0]);
    }

    [Test]
    public async Task FindComponents_ShouldReturnType_WhenItInheritsFromTypeWhichImplementsIComponent()
    {
        // Arrange.
        IReadOnlyList<Type> types = [typeof(InheritedComponentType)];

        // Act.
        var componentTypes = types.FindComponents().ToArray();

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(componentTypes).HasCount().EqualToOne();
        await Assert.That(componentTypes[0]).IsSameReferenceAs(types[0]);
    }
}

file class ComponentType : IComponent
{
    public void Attach(RenderHandle renderHandle)
    {
        // Empty.
    }

    public Task SetParametersAsync(ParameterView parameters) => Task.CompletedTask;
}

file sealed class InheritedComponentType : ComponentType;
