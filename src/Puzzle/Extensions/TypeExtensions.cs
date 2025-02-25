using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;

namespace Puzzle.Extensions;

internal static class TypeExtensions
{
    public static object? GetServiceKey(this Type type)
    {
        var baseAttribute = type.GetCustomAttribute(typeof(ServiceKeyAttribute<>), inherit: false);

        if (baseAttribute is null)
            return null;

        var keyAttribute = baseAttribute as ServiceKeyAttributeBase;
        return keyAttribute?.Key;
    }

    public static bool TryFindService(
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
