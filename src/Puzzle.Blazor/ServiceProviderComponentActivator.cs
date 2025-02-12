using Microsoft.AspNetCore.Components;

namespace Puzzle.Blazor;

internal sealed class ServiceProviderComponentActivator : IComponentActivator
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceProviderComponentActivator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IComponent CreateInstance(Type componentType)
    {
        var instance = _serviceProvider.GetService(componentType);

        if (instance is not null)
            return (IComponent)instance;

        instance = Activator.CreateInstance(componentType);
        return (IComponent)instance!;
    }
}
