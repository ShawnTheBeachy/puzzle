using Microsoft.Extensions.DependencyInjection;

namespace Puzzle;

internal delegate IServiceProvider Bootstrapper(
    IServiceCollection serviceCollection,
    IServiceProvider serviceProvider,
    BootstrapperNext next
);

internal delegate IServiceProvider BootstrapperNext(
    IServiceCollection serviceCollection,
    IServiceProvider serviceProvider
);
