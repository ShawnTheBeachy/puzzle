using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Puzzle;

public static class DependencyInjection
{
    public static IServiceCollection AddPlugins(
        this IHostApplicationBuilder applicationBuilder,
        Action<PuzzleConfiguration>? configure = null
    ) => applicationBuilder.Services.AddPlugins(applicationBuilder.Configuration, configure);

    public static IServiceCollection AddPlugins(
        this IServiceCollection serviceCollection,
        IConfiguration configuration,
        Action<PuzzleConfiguration>? configure = null
    )
    {
        if (configure is not null)
            configure(new PuzzleConfiguration([], configuration, serviceCollection));

        return serviceCollection;
    }
}
