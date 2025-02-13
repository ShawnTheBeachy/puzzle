using System.Reflection;
using Puzzle.Abstractions;

namespace Puzzle;

public sealed class Plugin
{
    public ITypeProvider AllTypes { get; }
    public Assembly Assembly { get; }
    public Type? BootstrapperType { get; }

    public Plugin(ITypeProvider allTypes, Assembly assembly, Type bootstrapperType = null!)
    {
        AllTypes = allTypes;
        Assembly = assembly;
        BootstrapperType = bootstrapperType;
    }
}
