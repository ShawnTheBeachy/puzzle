using Microsoft.Extensions.Logging;
using Puzzle.Tests.Unit.TestPlugin;

namespace Puzzle.Tests.Unit;

public sealed class PluginLoaderTests
{
    private static string? _pluginsPath;

    [Before(Class)]
    public static void CopyPluginDll()
    {
        var assembly = typeof(ExportedMetadata).Assembly;
        var assemblyDir = new FileInfo(assembly.Location).DirectoryName!;
        var pluginsDir = Path.Combine(assemblyDir, "plugins");
        var dllName = new FileInfo(assembly.Location).Name;
        var dll = Path.Combine(assemblyDir, dllName);
        var pluginDir = Path.Combine(pluginsDir, "tests");
        Directory.CreateDirectory(pluginDir);
        File.Copy(dll, Path.Combine(pluginDir, dllName), true);
        _pluginsPath = pluginsDir;
    }

    [Test]
    public async Task Plugin_ShouldBeLoaded_WhenItExistsInLocationSpecifiedInOptions()
    {
        // Arrange.
        var options = new PluginOptions { Locations = [_pluginsPath!] };

        // Act.
        var sut = new PluginLoader(options, Substitute.For<ILogger<PluginLoader>>());

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(sut.Plugins()).HasCount().EqualToOne();
        await Assert.That(sut.Plugins()[0].Id).IsEqualTo(new ExportedMetadata().Id);
    }
}
