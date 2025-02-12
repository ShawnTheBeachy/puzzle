using Microsoft.Extensions.DependencyInjection;

namespace Puzzle.Bootstrap;

public static class PluginExtensions
{
    public static IServiceProvider Bootstrap(
        this Plugin plugin,
        IServiceCollection serviceCollection,
        IServiceProvider serviceProvider
    ) => serviceCollection.BuildServiceProvider();
}
