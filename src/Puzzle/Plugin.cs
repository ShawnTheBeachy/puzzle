using Puzzle.Abstractions;

namespace Puzzle;

public sealed class Plugin
{
    public ITypeProvider AllTypes { get; }

    public Plugin(ITypeProvider allTypes)
    {
        AllTypes = allTypes;
    }
}
