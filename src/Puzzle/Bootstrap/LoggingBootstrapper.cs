﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Puzzle.Abstractions;

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
            serviceCollection.Replace(ServiceDescriptor.Singleton<ILoggerFactory>(loggerFactory));

        return next(serviceCollection, serviceProvider);
    }
}
