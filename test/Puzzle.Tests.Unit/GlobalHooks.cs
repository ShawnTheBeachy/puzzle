using Puzzle.Tests.Unit.TestPluginA;

namespace Puzzle.Tests.Unit;

internal static class GlobalHooks
{
    public static string PluginsPath { get; private set; } = "";

    [Before(TestSession)]
    public static void CopyPluginDll()
    {
        var assembly = typeof(ExportedMetadata).Assembly;
        var assemblyDir = new FileInfo(assembly.Location).DirectoryName!;
        var pluginsDir = Path.Combine(assemblyDir, "plugins");
        var dllName = new FileInfo(assembly.Location).Name;
        var dll = Path.Combine(assemblyDir, dllName);
        var pluginDir = Path.Combine(pluginsDir, "tests");

        if (Directory.Exists(pluginDir))
            Directory.Delete(pluginDir, true);

        Directory.CreateDirectory(pluginDir);
        File.Copy(dll, Path.Combine(pluginDir, dllName), true);
        PluginsPath = pluginsDir;
    }
}
