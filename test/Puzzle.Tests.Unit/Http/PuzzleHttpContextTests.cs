using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Puzzle.Http;

namespace Puzzle.Tests.Unit.Http;

public sealed class PuzzleHttpContextTests
{
    [Test]
    public async Task Abort_ShouldAbortBaseRequest()
    {
        // Arrange.
        var cancellation = new CancellationTokenSource();
        var @base = Substitute.For<HttpContext>();
        @base.When(x => x.Abort()).Do(_ => cancellation.Cancel());
        var sut = new PuzzleHttpContext(@base);

        // Act.
        sut.Abort();

        // Assert.
        await Assert.That(cancellation.IsCancellationRequested).IsTrue();
    }

    [Test]
    public async Task GetConnection_ShouldReturnBaseConnection()
    {
        // Arrange.
        var connectionInfo = Substitute.For<ConnectionInfo>();
        var @base = Substitute.For<HttpContext>();
        @base.Connection.Returns(connectionInfo);

        // Act.
        var sut = new PuzzleHttpContext(@base);

        // Assert.
        await Assert.That(sut.Connection).IsSameReferenceAs(connectionInfo);
    }

    [Test]
    public async Task GetFeatures_ShouldReturnBaseFeatures()
    {
        // Arrange.
        var features = Substitute.For<IFeatureCollection>();
        var @base = Substitute.For<HttpContext>();
        @base.Features.Returns(features);

        // Act.
        var sut = new PuzzleHttpContext(@base);

        // Assert.
        await Assert.That(sut.Features).IsSameReferenceAs(features);
    }

    [Test]
    public async Task GetItems_ShouldReturnBaseItems()
    {
        // Arrange.
        var items = new Dictionary<object, object?>();
        var @base = Substitute.For<HttpContext>();
        @base.Items.Returns(items);

        // Act.
        var sut = new PuzzleHttpContext(@base);

        // Assert.
        await Assert.That(sut.Items).IsSameReferenceAs(items);
    }

    [Test]
    public async Task GetRequest_ShouldReturnBaseRequest()
    {
        // Arrange.
        var request = Substitute.For<HttpRequest>();
        var @base = Substitute.For<HttpContext>();
        @base.Request.Returns(request);

        // Act.
        var sut = new PuzzleHttpContext(@base);

        // Assert.
        await Assert.That(sut.Request).IsSameReferenceAs(request);
    }

    [Test]
    public async Task GetRequestAborted_ShouldReturnBaseRequestAborted()
    {
        // Arrange.
        var cancellation = new CancellationTokenSource();
        var @base = Substitute.For<HttpContext>();
        @base.RequestAborted.Returns(cancellation.Token);

        // Act.
        var sut = new PuzzleHttpContext(@base);

        // Assert.
        await Assert.That(sut.RequestAborted).IsEqualTo(cancellation.Token);
    }

    [Test]
    public async Task GetRequestServices_ShouldNotReturnBaseRequestServices()
    {
        // Arrange.
        var services = Substitute.For<IServiceProvider>();
        var @base = Substitute.For<HttpContext>();
        @base.RequestServices.Returns(services);

        // Act.
        var sut = new PuzzleHttpContext(@base);

        // Assert.
        await Assert.That(sut.RequestServices).IsNotSameReferenceAs(services);
    }

    [Test]
    public async Task GetResponse_ShouldReturnBaseResponse()
    {
        // Arrange.
        var response = Substitute.For<HttpResponse>();
        var @base = Substitute.For<HttpContext>();
        @base.Response.Returns(response);

        // Act.
        var sut = new PuzzleHttpContext(@base);

        // Assert.
        await Assert.That(sut.Response).IsSameReferenceAs(response);
    }

    [Test]
    public async Task GetSession_ShouldReturnBaseSession()
    {
        // Arrange.
        var session = Substitute.For<ISession>();
        var @base = Substitute.For<HttpContext>();
        @base.Session.Returns(session);

        // Act.
        var sut = new PuzzleHttpContext(@base);

        // Assert.
        await Assert.That(sut.Session).IsSameReferenceAs(session);
    }

    [Test]
    public async Task GetTraceIdentifier_ShouldReturnBaseTraceIdentifier()
    {
        // Arrange.
        const string traceId = "trace-id";
        var @base = Substitute.For<HttpContext>();
        @base.TraceIdentifier.Returns(traceId);

        // Act.
        var sut = new PuzzleHttpContext(@base);

        // Assert.
        await Assert.That(sut.TraceIdentifier).IsSameReferenceAs(traceId);
    }

    [Test]
    public async Task GetUser_ShouldReturnBaseUser()
    {
        // Arrange.
        var user = new ClaimsPrincipal();
        var @base = Substitute.For<HttpContext>();
        @base.User.Returns(user);

        // Act.
        var sut = new PuzzleHttpContext(@base);

        // Assert.
        await Assert.That(sut.User).IsSameReferenceAs(user);
    }

    [Test]
    public async Task GetWebSockets_ShouldReturnBaseWebSockets()
    {
        // Arrange.
        var webSockets = Substitute.For<WebSocketManager>();
        var @base = Substitute.For<HttpContext>();
        @base.WebSockets.Returns(webSockets);

        // Act.
        var sut = new PuzzleHttpContext(@base);

        // Assert.
        await Assert.That(sut.WebSockets).IsSameReferenceAs(webSockets);
    }

    [Test]
    public async Task SetItems_ShouldSetBaseItems()
    {
        // Arrange.
        var @base = Substitute.For<HttpContext>();
        var sut = new PuzzleHttpContext(@base);

        // Act.
        var items = new Dictionary<object, object?>();
        sut.Items = items;

        // Assert.
        await Assert.That(@base.Items).IsSameReferenceAs(items);
    }

    [Test]
    public async Task SetRequestAborted_ShouldSetBaseRequestAborted()
    {
        // Arrange.
        var @base = Substitute.For<HttpContext>();
        var sut = new PuzzleHttpContext(@base);

        // Act.
        var cancellation = CancellationToken.None;
        sut.RequestAborted = cancellation;

        // Assert.
        await Assert.That(@base.RequestAborted).IsEqualTo(cancellation);
    }

    [Test]
    public async Task SetRequestServices_ShouldNotSetBaseRequestServices()
    {
        // Arrange.
        var @base = Substitute.For<HttpContext>();
        var sut = new PuzzleHttpContext(@base);

        // Act.
        var services = Substitute.For<IServiceProvider>();
        sut.RequestServices = services;

        // Assert.
        using var asserts = Assert.Multiple();
        await Assert.That(@base.RequestServices).IsNotSameReferenceAs(services);
        await Assert.That(sut.RequestServices).IsSameReferenceAs(services);
    }

    [Test]
    public async Task SetSession_ShouldSetBaseSession()
    {
        // Arrange.
        var @base = Substitute.For<HttpContext>();
        var sut = new PuzzleHttpContext(@base);

        // Act.
        var session = Substitute.For<ISession>();
        sut.Session = session;

        // Assert.
        await Assert.That(sut.Session).IsSameReferenceAs(session);
    }

    [Test]
    public async Task SetTraceIdentifier_ShouldSetBaseTraceIdentifier()
    {
        // Arrange.
        var @base = Substitute.For<HttpContext>();
        var sut = new PuzzleHttpContext(@base);

        // Act.
        const string traceId = "trace-id";
        sut.TraceIdentifier = traceId;

        // Assert.
        await Assert.That(sut.TraceIdentifier).IsSameReferenceAs(traceId);
    }

    [Test]
    public async Task SetUser_ShouldSetBaseUser()
    {
        // Arrange.
        var @base = Substitute.For<HttpContext>();
        var sut = new PuzzleHttpContext(@base);

        // Act.
        var user = new ClaimsPrincipal();
        sut.User = user;

        // Assert.
        await Assert.That(sut.User).IsSameReferenceAs(user);
    }
}
