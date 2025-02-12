using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

        return config;
    }
}
