using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Puzzle.Http;

internal sealed class PuzzleHttpContext : HttpContext
{
    private readonly HttpContext _baseContext;

    public PuzzleHttpContext(HttpContext baseContext)
    {
        _baseContext = baseContext;
    }

    public override void Abort() => _baseContext.Abort();

    public override ConnectionInfo Connection => _baseContext.Connection;
    public override IFeatureCollection Features => _baseContext.Features;
    public override IDictionary<object, object?> Items
    {
        get => _baseContext.Items;
        set => _baseContext.Items = value;
    }
    public override HttpRequest Request => _baseContext.Request;
    public override CancellationToken RequestAborted
    {
        get => _baseContext.RequestAborted;
        set => _baseContext.RequestAborted = value;
    }
    public override IServiceProvider RequestServices { get; set; } = null!;
    public override HttpResponse Response => _baseContext.Response;

    public override ISession Session
    {
        get => _baseContext.Session;
        set => _baseContext.Session = value;
    }

    public override string TraceIdentifier
    {
        get => _baseContext.TraceIdentifier;
        set => _baseContext.TraceIdentifier = value;
    }

    public override ClaimsPrincipal User
    {
        get => _baseContext.User;
        set => _baseContext.User = value;
    }

    public override WebSocketManager WebSockets => _baseContext.WebSockets;
}
