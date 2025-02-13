using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Puzzle.Bootstrap;

internal static class LoggingBootstrapper
{
    public static IServiceProvider Bootstrap(
        IServiceCollection serviceCollection,
        IServiceProvider serviceProvider,
        BootstrapperNext next
    )
    {
        serviceCollection.AddLogging();
        var loggerFactory = serviceProvider.GetService<ILoggerFactory>();

        if (loggerFactory is not null)
            serviceCollection.Replace(ServiceDescriptor.Singleton(loggerFactory));

        return next(serviceCollection, serviceProvider);
    }
}
