using Microsoft.Extensions.DependencyInjection;

namespace Puzzle;

internal delegate IServiceProvider Bootstrapper(
    IServiceCollection serviceCollection,
    IServiceProvider serviceProvider
);
