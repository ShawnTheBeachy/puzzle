using System.Reflection;
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
}
