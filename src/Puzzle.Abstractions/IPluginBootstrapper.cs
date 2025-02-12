using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Puzzle.Abstractions;

public interface IPluginBootstrapper
{
    public IServiceCollection Bootstrap(IServiceCollection services, IConfiguration configuration);
}
