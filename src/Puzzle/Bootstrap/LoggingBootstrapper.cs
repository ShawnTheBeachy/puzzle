using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Puzzle.Abstractions;

namespace Puzzle.Bootstrap;

internal sealed class LoggingBootstrapper : IBootstrapperInternal
{
    public IServiceProvider Bootstrap(
        IServiceCollection serviceCollection,
        IServiceProvider serviceProvider,
        Bootstrapper next
    )
    {
        serviceCollection.AddLogging();
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

        if (loggerFactory is not null)
            serviceCollection.Replace(ServiceDescriptor.Singleton<ILoggerFactory>(loggerFactory));

        return next(serviceCollection, serviceProvider);
    }
}
