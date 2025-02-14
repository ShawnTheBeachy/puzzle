using System.Reflection;
using System.Runtime.Loader;
using Puzzle.Options;

namespace Puzzle;

internal static class AssemblyScanning
{
    public static IEnumerable<Assembly> ScanAssemblies(PuzzleOptions options) =>
        ScanAssemblyFiles(options)
            .Select(assemblyFile =>
            {
                var loadContext = options.IsolatePlugins
                    ? new PluginLoadContext(assemblyFile.FullName)
                    : AssemblyLoadContext.Default;
                return loadContext.LoadFromAssemblyPath(assemblyFile.FullName);
            });

    private static IEnumerable<FileInfo> ScanAssemblyFiles(PuzzleOptions options) =>
        options
            .Locations.Select(location => new DirectoryInfo(location))
            .SelectMany(directory =>
            {
                try
                {
                    return directory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly);
                }
                catch (DirectoryNotFoundException)
                {
                    return [];
                }
            })
            .SelectMany(directory =>
                directory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly)
            );
}
