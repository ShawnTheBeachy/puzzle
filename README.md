# Puzzle
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ShawnTheBeachy/puzzle/publish-nuget.yml)
![NuGet Version](https://img.shields.io/nuget/v/asdfDEV.Puzzle)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/ShawnTheBeachy/puzzle)
![GitHub License](https://img.shields.io/github/license/ShawnTheBeachy/puzzle)
![GitHub repo size](https://img.shields.io/github/repo-size/ShawnTheBeachy/puzzle)

Puzzle is a simple plugin system for .NET. It does not aim to be a fancy, best-to-ever-do-it package. It is for projects where *simple*, *basic* plugin support is needed.

Plugins are loaded in isolation; that is, they are loaded in their own `AssemblyLoadContext` and are isolated from the rest of the application services. Plugin services are injected alongside your application services so that you can resolve them from your DI container without any extra work. However, a plugin service will not be able to resolve your application services.

# Puzzle
Download from [NuGet](https://www.nuget.org/packages/asdfDEV.Puzzle).

This is the main package which should be used by plugin hosts. If you are writing a plugin, you will want the `Puzzle.Abstractions` package.

## Installation
Adding plugin support to your application is as easy as one line:

```c#
...
builder.Services.AddPlugins(builder.Configuration);
...
```

Alternatively, for Puzzle to automatically pass itself the configuration:

```c#
...
builder.AddPlugins();
...
```

## Configuration
Puzzle configuration options should be nested under a `Plugins` section.

```json
{
  "Plugins": {
    "Locations": [
      "/path/to/plugins"
    ]
  },
  "StartupThreshold": "00:00:01.0000"
}
```

### `Locations` (optional)
The locations in which Puzzle will look for plugins. Each plugin must be nested one level deep inside its own folder. For example:

    .
    ├── ...
    ├── plugins                    # The directory which you will include in `Locations`
    │   ├── com.company.pluginA    # The sub-directory for a plugin
    |   |   |── PluginA.dll
    |   |   └── ...                # Other plugin files
    │   ├── com.company.pluginB    # The sub-directory for another plugin
    |   |   |── PluginB.dll
    |   |   └── ...                # Other plugin files
    └── ...

### `StartupThreshold` (optional)
A target time which plugin initialization should not exceed during application startup. If plugin initialization exceeds this time, a warning will be logged.

# Puzzle.Abstractions
Download from [NuGet](https://www.nuget.org/packages/asdfDEV.Puzzle.Abstractions).

If you are creating a plugin, this is the package for you.

## `Service<>`
For a plugin service to be registered you must mark it with the `Service<>` attribute. NOTE: This should *only* be used for services which are implementing an abstraction. For example, if your target host application is expecting an implementation of `IDataSource` and you are implementing it via `MyDataSource`, you should mark `MyDataSource` with `Service<IDataSource`. If `MyDataSource` relies on another service in your project, e.g. `MyConnectionBuilder`, you should *not* mark `MyConnectionBuilder` with the `Service<>` attribute. 

The `Service<>` attribute takes a `Lifetime` argument. This can be `Scoped`, `Singleton`, or `Transient`.

Your service must be `public` for Puzzle to pick it up.

## Metadata
Your plugin must export an implementation of `IPluginMetadata`. If you do not export an implementation, your plugin will not be loaded.

```c#
public sealed class MyPluginMetadata : IPluginMetadata
{
  public string Id => "com.company.plugin";
  public string Name => "My Plugin";
}
```

## Bootstrapping (optional)
If your plugin requires services to operate you can inject them via an exported `IPluginBootstrapper` implementation. Only one implementation is supported per plugin.

```c#
public sealed class MyPluginBootstrapper : IPluginBootstrapper
{
  public IServiceCollection Bootstrap(IServiceCollection services, IConfiguration configuration)
  {
    services.AddTransient<MyConnectionBuilder>(configuration.GetRequiredSection("Connections"));
    return services;
  }
}
```

The `IConfiguration` object which is passed in has two sources:
  1. Environment variables
  2. (Optional) A `settings.json` file located at the root of your plugin.

# Puzzle.Blazor
Download from [NuGet](https://www.nuget.org/packages/asdfDEV.Puzzle.Blazor).
If you want to support Blazor components in your application host, you will need to install `Puzzle.Blazor`.

```c#
...
builder.AddPlugins(config => config.AddBlazor());
...
app.MapRazorComponents<App>()
  .AddPluginComponents(app)
  ...;
...
```

Puzzle will automatically register all exported `IComponent` instances from plugins. When components from these plugins are used, Puzzle will activate the components using the plugin's DI container instead of the host application's DI container.

In order for DI to work the component must use constructor injection; NOT property injection via `@inject`. The notable exceptions to this rule are `NavigationManager` and `IJsRuntime`.
