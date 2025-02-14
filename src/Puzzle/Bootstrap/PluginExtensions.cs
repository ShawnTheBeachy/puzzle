using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;

namespace Puzzle.Bootstrap;

internal static class PluginExtensions
{
    public static IServiceProvider Bootstrap(
        this Plugin plugin,
        IServiceCollection serviceCollection,
        IServiceProvider serviceProvider
    )
    {
        IReadOnlyList<Bootstrapper> bootstrappers =
        [
            HttpContextBootstrapper.Bootstrap,
            LoggingBootstrapper.Bootstrap,
            new PluginBootstrapper(plugin).Bootstrap,
        ];
        var bootstrap = bootstrappers.Aggregate(
            (BootstrapperNext)((sc, _) => sc.BuildServiceProvider()),
            (next, bootstrapper) => (sc, sp) => bootstrapper(sc, sp, next)
        );
        return bootstrap(serviceCollection, serviceProvider);
    }

    public static IServiceCollection Bootstrap(
        this Plugin plugin,
        IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        if (plugin.BootstrapperType is null)
            return serviceCollection;

        var bootstrapper = (IPluginBootstrapper)Activator.CreateInstance(plugin.BootstrapperType)!;
        serviceCollection = bootstrapper.Bootstrap(serviceCollection, configuration);
        return serviceCollection;
    }
}
