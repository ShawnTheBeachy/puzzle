using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Puzzle;

public static class DependencyInjection
{
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
