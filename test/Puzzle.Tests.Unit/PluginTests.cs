namespace Puzzle.Tests.Unit;

public sealed class PluginTests
{
    [Test]
    public async Task TryCreate_ShouldCreatePlugin_WhenAssemblyExportsPluginMetadata()
    {
        // Arrange.
        var assembly = typeof(PluginTests).Assembly;

        // Act.
        _ = Plugin.TryCreate(assembly, out var plugin);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(plugin).IsNotNull();
        await Assert.That(plugin!.Id).IsEqualTo(new ExportedMetadata().Id);
        await Assert.That(plugin.Name).IsEqualTo(new ExportedMetadata().Name);
        await Assert
            .That(plugin.Assembly.Location)
            .IsEqualTo(typeof(PluginTests).Assembly.Location);
        await Assert.That(plugin.AllTypes.GetTypes()).Contains(x => x == typeof(PluginTests));
        await Assert.That(plugin.BootstrapperType).IsEqualTo(typeof(ExportedBootstrapper));
    }

    [Test]
    public async Task TryCreate_ShouldReturnFalse_WhenAssemblyDoesNotExportPluginMetadata()
    {
        // Arrange.
        var assembly = typeof(string).Assembly;

        // Act.
        var success = Plugin.TryCreate(assembly, out var plugin);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(success).IsFalse();
        await Assert.That(plugin).IsNull();
    }

    [Test]
    public async Task TryCreate_ShouldReturnTrue_WhenAssemblyExportsPluginMetadata()
    {
        // Arrange.
        var assembly = typeof(PluginTests).Assembly;

        // Act.
        var success = Plugin.TryCreate(assembly, out _);

        // Assert.
        await Assert.That(success).IsTrue();
    }
}
