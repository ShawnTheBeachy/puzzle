using System.Reflection;
using Puzzle.Abstractions;

namespace Puzzle;

public sealed class Plugin
{
    public ITypeProvider AllTypes { get; }
    public Assembly Assembly { get; }

    public Plugin(ITypeProvider allTypes, Assembly assembly)
    {
        AllTypes = allTypes;
        Assembly = assembly;
    }
}
