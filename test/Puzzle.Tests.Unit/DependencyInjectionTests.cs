using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.Tests.Unit.TestPlugin;

namespace Puzzle.Tests.Unit;

public sealed class DependencyInjectionTests
{
    [Test]
    public async Task PluginService_ShouldBeRegistered_WhenItIsExportedFromPluginInLocationSpecifiedInOptions()
    {
        // Arrange.
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    {
                        $"{PluginOptions.SectionName}:{nameof(PluginOptions.Locations)}:0",
                        GlobalHooks.PluginsPath
                    },
                }
            )
            .Build();
        var services = new ServiceCollection();

        // Act.
        services.AddPlugins(configuration);

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert
            .That(services)
            .Contains(x =>
                x.ServiceType == typeof(ITuple) && x.Lifetime == ExportedService.Lifetime
            );
        var provider = services.BuildServiceProvider();
        await Assert
            .That(provider.GetService<ITuple>()?.GetType().FullName)
            .IsEqualTo(typeof(ExportedService).FullName);
    }
}
