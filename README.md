# Puzzle
![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ShawnTheBeachy/puzzle/publish-nuget.yml)
![NuGet Version](https://img.shields.io/nuget/v/asdfDEV.Puzzle)
![GitHub Issues or Pull Requests](https://img.shields.io/github/issues/ShawnTheBeachy/puzzle)
![GitHub License](https://img.shields.io/github/license/ShawnTheBeachy/puzzle)
![GitHub repo size](https://img.shields.io/github/repo-size/ShawnTheBeachy/puzzle)

Puzzle is a simple plugin system for .NET. It does not aim to be a fancy, best-to-ever-do-it package. It is for projects where *simple*, *basic* plugin support is needed.

Plugins are loaded in isolation; that is, they are loaded in their own `AssemblyLoadContext` and are isolated from the rest of the application services. Plugin services are injected alongside your application services so that you can resolve them from your DI container without any extra work. However, a plugin service will not be able to resolve your application services.

# Puzzle
Download from ![NuGet](https://www.nuget.org/packages/asdfDEV.Puzzle).

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
    |   |   └── ...
    │   ├── com.company.pluginB    # The sub-directory for another plugin
    |   |   |── PluginB.dll
    |   |   └── ...                # Other plugin files
    └── ...

### `StartupThreshold` (optional)
A target time which plugin initialization should not exceed during application startup. If plugin initialization exceeds this time, a warning will be logged.

# Puzzle.Abstractions
If you are creating a plugin, this is the package for you.

# Puzzle.Blazor
