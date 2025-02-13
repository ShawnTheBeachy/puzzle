using System.Reflection;

namespace Puzzle;

internal static class AssemblyScanning
{
    public static IEnumerable<Assembly> ScanAssemblies(PluginOptions options) =>
        ScanAssemblyFiles(options)
            .Select(assemblyFile =>
            {
                var loadContext = new PluginLoadContext(assemblyFile.FullName);
                return loadContext.LoadFromAssemblyPath(assemblyFile.FullName);
            });

    private static IEnumerable<FileInfo> ScanAssemblyFiles(PluginOptions options) =>
        options
            .Locations.Select(location => new DirectoryInfo(location))
            .SelectMany(directory =>
                directory.EnumerateDirectories("*", SearchOption.TopDirectoryOnly)
            )
            .SelectMany(directory =>
                directory.EnumerateFiles("*.dll", SearchOption.TopDirectoryOnly)
            );
}
