﻿using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Puzzle.Options;
using Puzzle.Tests.Unit.TestPlugin;

namespace Puzzle.Tests.Unit;

public sealed class PluginTests
{
    [Test]
    public async Task TryCreate_ShouldCreatePlugin_WhenAssemblyExportsPluginMetadata()
    {
        // Arrange.
        var assembly = typeof(ExportedMetadata).Assembly;

        // Act.
        _ = Plugin.TryCreate(assembly, Substitute.For<IConfiguration>(), out var plugin);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(plugin).IsNotNull();
        await Assert.That(plugin!.Id).IsEqualTo(new ExportedMetadata().Id);
        await Assert.That(plugin.Name).IsEqualTo(new ExportedMetadata().Name);
        await Assert
            .That(plugin.Version)
            .IsEqualTo(FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion);
        await Assert
            .That(plugin.Assembly.Location)
            .IsEqualTo(typeof(ExportedMetadata).Assembly.Location);
        await Assert.That(plugin.AllTypes.GetTypes()).Contains(x => x == typeof(ExportedMetadata));
        await Assert.That(plugin.BootstrapperType).IsEqualTo(typeof(ExportedBootstrapper));
    }

    [Test]
    public async Task TryCreate_ShouldReturnFalse_WhenAssemblyDoesNotExportPluginMetadata()
    {
        // Arrange.
        var assembly = typeof(string).Assembly;

        // Act.
        var success = Plugin.TryCreate(assembly, Substitute.For<IConfiguration>(), out var plugin);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(success).IsFalse();
        await Assert.That(plugin).IsNull();
    }

    [Test]
    public async Task TryCreate_ShouldReturnTrue_WhenAssemblyExportsPluginMetadata()
    {
        // Arrange.
        var assembly = typeof(ExportedMetadata).Assembly;

        // Act.
        var success = Plugin.TryCreate(assembly, Substitute.For<IConfiguration>(), out _);

        // Assert.
        await Assert.That(success).IsTrue();
    }

    [Test]
    public async Task TryCreate_ShouldSetIsDisabledToTrue_WhenPluginIsDisabledInConfiguration()
    {
        // Arrange.
        var assembly = typeof(ExportedMetadata).Assembly;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    { $"{new ExportedMetadata().Id}:{nameof(PluginOptions.Disabled)}", "true" },
                }
            )
            .Build();

        // Act.
        _ = Plugin.TryCreate(assembly, configuration, out var plugin);

        // Assert.
        await Assert.That(plugin!.IsDisabled).IsTrue();
    }

    [Test]
    public async Task TryCreate_ShouldSetPriority_WhenPriorityIsSetInConfiguration()
    {
        // Arrange.
        var assembly = typeof(ExportedMetadata).Assembly;
        const int priority = 5;
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{new ExportedMetadata().Id}:{nameof(PluginOptions.Priority)}",
                        priority.ToString()
                    },
                }
            )
            .Build();

        // Act.
        _ = Plugin.TryCreate(assembly, configuration, out var plugin);

        // Assert.
        await Assert.That(plugin!.Priority).IsEqualTo(priority);
    }
}
