using Microsoft.Extensions.DependencyInjection;

namespace Puzzle.Bootstrap;

public static class PluginExtensions
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
}
