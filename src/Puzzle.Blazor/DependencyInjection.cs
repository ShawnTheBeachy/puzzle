using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Puzzle.Abstractions;
using Puzzle.Bootstrap;

namespace Puzzle.Blazor;

public static class DependencyInjection
{
    public static PuzzleConfiguration AddBlazor(this PuzzleConfiguration config)
    {
        foreach (var plugin in config.Plugins)
        {
            var componentTypes = plugin.AllTypes.GetTypes().FindComponents();

            foreach (var type in componentTypes)
                config.Services.TryAdd(
                    ServiceDescriptor.Transient(
                        type,
                        sp =>
                        {
                            var services = new ServiceCollection();
                            services.AddTransient(type, type);
                            var provider = plugin.Bootstrap(services, sp);
                            return provider.GetRequiredService(type);
                        }
                    )
                );
        }

        config.Services.Replace(
            ServiceDescriptor.Singleton<IComponentActivator, ServiceProviderComponentActivator>()
        );
        return config;
    }

    public static RazorComponentsEndpointConventionBuilder AddPluginComponents(
        this RazorComponentsEndpointConventionBuilder builder,
        IHost host
    )
    {
        using var scope = host.Services.CreateScope();
        var pluginLoader = scope.ServiceProvider.GetRequiredService<IPluginLoader>();
        builder.AddAdditionalAssemblies(
            pluginLoader.Plugins().Select(plugin => plugin.Assembly).ToArray()
        );
        return builder;
    }
}
