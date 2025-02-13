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
