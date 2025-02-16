using Microsoft.AspNetCore.Components;

namespace Puzzle.Blazor;

internal static class ComponentDiscovery
{
    public static IEnumerable<Type> FindComponents(this IEnumerable<Type> types) =>
        types.Where(t => !t.IsAbstract).Where(t => t.IsAssignableTo(typeof(IComponent)));
}
