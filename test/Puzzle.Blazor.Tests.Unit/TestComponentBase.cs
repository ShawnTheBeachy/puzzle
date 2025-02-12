using Microsoft.AspNetCore.Components;

namespace Puzzle.Blazor.Tests.Unit;

internal abstract class TestComponentBase : IComponent
{
    public void Attach(RenderHandle renderHandle)
    {
        // Empty.
    }

    public Task SetParametersAsync(ParameterView parameters) => Task.CompletedTask;
}
