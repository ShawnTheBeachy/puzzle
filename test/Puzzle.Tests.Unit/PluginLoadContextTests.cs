namespace Puzzle.Tests.Unit;

public sealed class PluginLoadContextTests
{
    [Test]
    public async Task Load_ShouldReturnAssembly_WhenItExistsInLocation()
    {
        // Arrange.
        var directory = new FileInfo(
            typeof(PluginLoadContextTests).Assembly.Location
        ).DirectoryName;
        var sut = new PluginLoadContext(directory!);

        // Act.
        var assembly = sut.LoadFromAssemblyName(typeof(PluginLoadContextTests).Assembly.GetName());

        // Assert.
        await Assert.That(assembly).IsNotNull();
    }

    [Test]
    public async Task Load_ShouldReturnNull_WhenItDoesNotExistInLocation()
    {
        // Arrange.
        var directory = new FileInfo(typeof(PluginLoadContext).Assembly.Location).DirectoryName;
        var sut = new PluginLoadContext(directory!);

        // Act.
        var assembly = sut.LoadFromAssemblyName(typeof(PluginLoadContextTests).Assembly.GetName());

        // Assert.
        await Assert.That(assembly).IsNotNull();
    }
}
