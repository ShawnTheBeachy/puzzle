namespace Puzzle.Abstractions;

public abstract class ServiceKeyAttributeBase(object key) : Attribute
{
    public object Key => key;
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class ServiceKeyAttribute<T>(T key) : ServiceKeyAttributeBase(key)
    where T : notnull;
