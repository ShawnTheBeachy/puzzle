using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;

namespace Puzzle.Bootstrap;

internal sealed class PluginBootstrapper
{
    private readonly Plugin _plugin;

    public PluginBootstrapper(Plugin plugin)
    {
        _plugin = plugin;
    }

    public IServiceProvider Bootstrap(
        IServiceCollection serviceCollection,
        IServiceProvider serviceProvider,
        BootstrapperNext next
    )
    {
        if (_plugin.BootstrapperType is null)
            return next(serviceCollection, serviceProvider);

        var configurationBuilder = new ConfigurationBuilder();
        configurationBuilder.SetBasePath(Path.GetDirectoryName(_plugin.Assembly.Location)!);
        var configuration = configurationBuilder
            .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var bootstrapper = (IPluginBootstrapper)Activator.CreateInstance(_plugin.BootstrapperType)!;
        serviceCollection = bootstrapper.Bootstrap(serviceCollection, configuration);
        return next(serviceCollection, serviceProvider);
    }
}
