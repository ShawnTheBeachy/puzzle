using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Puzzle.Abstractions;
using Puzzle.Bootstrap;
using Puzzle.Extensions;
using Puzzle.Options;

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
        configuration = configuration.GetSection(PuzzleOptions.SectionName);
        serviceCollection.AddOptions<PuzzleOptions>().Bind(configuration);

        using var loggingServices = serviceCollection.GetLoggingServices();
        var logger = loggingServices.GetRequiredService<ILogger<PluginLoader>>();
        var startupTimer = new Stopwatch();
        startupTimer.Start();

        var loader = new PluginLoader(configuration, logger);
        serviceCollection.AddSingleton<IPluginLoader>(loader);

        serviceCollection.AddPlugins(loader.Plugins(), configuration);

        configure?.Invoke(
            new PuzzleConfiguration(loader.Plugins(), configuration, serviceCollection)
        );

        startupTimer.Stop();
        CheckStartupThreshold(startupTimer.Elapsed, loader.StartupThreshold, logger);
        return serviceCollection;
    }

    internal static IServiceCollection AddPlugins(
        this IServiceCollection serviceCollection,
        IReadOnlyList<Plugin> plugins,
        IConfiguration configuration
    )
    {
        foreach (
            var plugin in plugins
                .OrderBy(x => x.Priority is not null)
                .ThenByDescending(x => x.Priority)
        )
            HandlePlugin(plugin, serviceCollection, configuration);

        return serviceCollection;
    }

    private static void CheckStartupThreshold(TimeSpan elapsed, TimeSpan? threshold, ILogger logger)
    {
        if (threshold is null)
            return;

        if (elapsed < threshold)
            return;

        logger.StartupThresholdWarning(elapsed);
    }

    private static ServiceProvider GetLoggingServices(this IServiceCollection serviceCollection) =>
        serviceCollection.AddLogging().BuildServiceProvider();

    private static void HandlePlugin(
        Plugin plugin,
        IServiceCollection serviceCollection,
        IConfiguration configuration
    )
    {
        var options = configuration.Get<PuzzleOptions>();

        if (plugin.IsDisabled)
            return;

        if (!(options?.IsolatePlugins ?? true))
            serviceCollection = plugin.Bootstrap(
                serviceCollection,
                (IConfiguration?)configuration.GetSection(plugin.Id).GetSection("Options")
                    ?? new ConfigurationBuilder().Build()
            );

        foreach (var type in plugin.AllTypes.GetTypes())
        {
            if (
                !type.TryFindService(out var serviceType, out var lifetime, out var key)
                || serviceType is null
                || lifetime is null
            )
                continue;

            serviceCollection.AddService(serviceType, type, lifetime.Value, key, plugin, options);
        }
    }

    private static bool TryFindService(
        this Type type,
        out Type? serviceType,
        out ServiceLifetime? lifetime,
        out object? key
    )
    {
        serviceType = null;
        lifetime = null;
        key = null;
        var baseAttribute = type.GetCustomAttribute(typeof(ServiceAttribute<>), inherit: false);

        if (baseAttribute is null)
            return false;

        serviceType = baseAttribute.GetType().GetGenericArguments()[0];

        if (!serviceType.IsAssignableFrom(type))
            throw new Exception(Messages.ServiceNotImplemented(type, serviceType));

        lifetime = ((ServiceAttribute)baseAttribute).Lifetime;
        key = type.GetServiceKey();
        return true;
    }

    internal static class Messages
    {
        public static string ServiceNotImplemented(Type implementationType, Type serviceType) =>
            $"{implementationType} is registered as a plugin for {serviceType} but {implementationType} does not implement {serviceType}.";
    }
}
