namespace Puzzle.Abstractions;

public interface ITypeProvider
{
    IEnumerable<Type> GetTypes();
}
