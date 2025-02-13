using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Puzzle.Abstractions;
using Puzzle.Http;

namespace Puzzle.Bootstrap;

internal sealed class HttpContextBootstrapper : IBootstrapperInternal
{
    public IServiceProvider Bootstrap(
        IServiceCollection serviceCollection,
        IServiceProvider serviceProvider,
        Bootstrapper next
    )
    {
        var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
        var httpContext = httpContextAccessor?.HttpContext is null
            ? null
            : new PuzzleHttpContext(httpContextAccessor.HttpContext);

        if (httpContextAccessor is not null)
            serviceCollection.AddSingleton<IHttpContextAccessor>(
                new HttpContextAccessor { HttpContext = httpContext }
            );

        var constructedProvider = next(serviceCollection, serviceProvider);

        if (httpContext is not null)
            httpContext.RequestServices = constructedProvider;

        return constructedProvider;
    }
}
