using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;
using Puzzle.Tests.Unit.TestPlugin;

namespace Puzzle.Tests.Unit;

public sealed class ServiceDependencyInjectionTests
{
    [Test]
    public async Task Service_ShouldNotBeDisposed_WhenItIsResolved()
    {
        // Arrange.
        var plugin = new Plugin(
            Substitute.For<ITypeProvider>(),
            typeof(ServiceDependencyInjectionTests).Assembly,
            new ExportedMetadata()
        );

        var services = new ServiceCollection();
        services.AddService(
            typeof(DisposableService),
            typeof(DisposableService),
            ServiceLifetime.Singleton,
            null,
            plugin,
            null
        );

        // Act.
        var provider = services.BuildServiceProvider();
        var resolved = provider.GetRequiredService<DisposableService>();

        // Assert.
        await Assert.That(resolved.IsDisposed).IsFalse();
    }
}

file sealed class DisposableService : IDisposable
{
    public bool IsDisposed { get; private set; }

    public void Dispose() => IsDisposed = true;
}
