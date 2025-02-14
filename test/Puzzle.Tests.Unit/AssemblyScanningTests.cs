using Microsoft.Extensions.Logging;
using Puzzle.Options;

namespace Puzzle.Tests.Unit;

public sealed class AssemblyScanningTests
{
    [Test]
    public async Task ScanAssemblies_ShouldLogWarning_WhenLocationDoesNotExist()
    {
        // Arrange.
        const string location = "/my/fake/path";
        var options = new PuzzleOptions { Locations = [location, GlobalHooks.PluginsPath] };
        var logger = new TestableLogger<AssemblyScanningTests>();

        // Act.
        _ = AssemblyScanning.ScanAssemblies(options, logger).ToArray();

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(logger.LogMessages).HasCount().EqualToOne();
        await Assert
            .That(logger.LogMessages)
            .Contains(x =>
                x.LogLevel == LogLevel.Warning
                && x.Message
                    == Logging.Messages.PluginLocationNotExists.Replace("{Location}", location)
            );
    }

    [Test]
    public async Task ScanAssemblies_ShouldReturnAssembly_WhenItIsInLocationSpecifiedInOptions()
    {
        // Arrange.
        var options = new PuzzleOptions
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
        var assemblies = AssemblyScanning
            .ScanAssemblies(options, Substitute.For<ILogger>())
            .ToArray();

        // Assert.
        await Assert
            .That(assemblies)
            .Contains(x => x.Location == typeof(AssemblyScanningTests).Assembly.Location);
    }

    [Test]
    public async Task ScanAssemblies_ShouldSkipLocation_WhenItDoesNotExist()
    {
        // Arrange.
        var options = new PuzzleOptions { Locations = ["/my/fake/path"] };

        // Act.
        var assemblies = AssemblyScanning
            .ScanAssemblies(options, Substitute.For<ILogger>())
            .ToArray();

        // Assert.
        await Assert.That(assemblies).IsEmpty();
    }
}
