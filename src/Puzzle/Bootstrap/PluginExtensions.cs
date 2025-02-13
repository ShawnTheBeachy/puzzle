using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;

namespace Puzzle.Bootstrap;

public static class PluginExtensions
{
    public static IServiceProvider Bootstrap(
        this Plugin plugin,
        IServiceCollection serviceCollection,
        IServiceProvider serviceProvider
    )
    {
        IReadOnlyList<IBootstrapperInternal> bootstrappers =
        [
            new HttpContextBootstrapper(),
            new LoggingBootstrapper(),
        ];
        var bootstrap = bootstrappers.Aggregate(
            (Bootstrapper)((sc, _) => sc.BuildServiceProvider()),
            (next, bootstrapper) => (sc, sp) => bootstrapper.Bootstrap(sc, sp, next)
        );
        return bootstrap(serviceCollection, serviceProvider);
    }
}
