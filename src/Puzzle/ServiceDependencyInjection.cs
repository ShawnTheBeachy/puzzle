using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Puzzle.Abstractions;
using Puzzle.Bootstrap;
using Puzzle.Options;

namespace Puzzle;

internal static class ServiceDependencyInjection
{
    private static ServiceDescriptor GetIsolatedService(
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime,
        object? key,
        Plugin plugin
    )
    {
        return new ServiceDescriptor(serviceType, key, ImplementationFactory, lifetime);

        object ImplementationFactory(IServiceProvider sp, object? keyUsed)
        {
            var services = new ServiceCollection().Add(
                new ServiceDescriptor(implementationType, key, implementationType, lifetime)
            );
            var provider = plugin.Bootstrap(services, sp);
            var resolved = key is null
                ? provider.GetRequiredService(implementationType)
                : provider.GetRequiredKeyedService(implementationType, key);
            return resolved;
        }
    }

    public static void AddService(
        this IServiceCollection serviceCollection,
        Type serviceType,
        Type implementationType,
        ServiceLifetime lifetime,
        object? key,
        Plugin plugin,
        PuzzleOptions? options
    )
    {
        var isolate = options?.IsolatePlugins ?? new PuzzleOptions().IsolatePlugins;
        var isHostedService = implementationType.IsAssignableTo(typeof(IHostedService));
        var isExclusive = !isHostedService && IsExclusive(serviceType);

        if (isHostedService)
        {
            lifetime = ServiceLifetime.Singleton;
            serviceType = typeof(IHostedService);
        }

        var serviceDescriptor = isolate
            ? GetIsolatedService(serviceType, implementationType, lifetime, key, plugin)
            : new ServiceDescriptor(serviceType, key, implementationType, lifetime);

        if (isExclusive)
            serviceCollection.Replace(serviceDescriptor);
        else
            serviceCollection.Add(serviceDescriptor);
    }

    private static bool IsExclusive(Type serviceType)
    {
        var exclusiveAttribute = serviceType.GetCustomAttribute<ExclusiveAttribute>();
        return exclusiveAttribute is not null;
    }
}
