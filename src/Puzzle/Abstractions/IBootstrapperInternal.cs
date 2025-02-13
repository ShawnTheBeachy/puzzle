using Microsoft.Extensions.DependencyInjection;

namespace Puzzle.Abstractions;

internal interface IBootstrapperInternal
{
    IServiceProvider Bootstrap(
        IServiceCollection serviceCollection,
        IServiceProvider serviceProvider,
        Bootstrapper next
    );
}
