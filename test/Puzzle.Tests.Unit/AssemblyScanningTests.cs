namespace Puzzle.Tests.Unit;

public sealed class AssemblyScanningTests
{
    [Test]
    public async Task ScanAssemblies_ShouldReturnAssembly_WhenItIsInLocationSpecifiedInOptions()
    {
        // Arrange.
        var options = new PluginOptions
        {
            Locations =
            [
                new DirectoryInfo(
                    new FileInfo(typeof(AssemblyScanningTests).Assembly.Location).DirectoryName!
                )
                    .Parent!
                    .FullName,
            ],
        };

        // Act.
        var assemblies = AssemblyScanning.ScanAssemblies(options).ToArray();

        // Assert.
        await Assert
            .That(assemblies)
            .Contains(x => x.Location == typeof(AssemblyScanningTests).Assembly.Location);
    }

    [Test]
    public async Task ScanAssemblies_ShouldSkipLocation_WhenItDoesNotExist()
    {
        // Arrange.
        var options = new PluginOptions { Locations = ["/usr/plugins"] };

        // Act.
        var assemblies = AssemblyScanning.ScanAssemblies(options).ToArray();

        // Assert.
        await Assert.That(assemblies).IsEmpty();
    }
}
