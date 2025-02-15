using System.Reflection;
using Puzzle.Abstractions;
using Puzzle.Tests.Unit.TestPlugin;

namespace Puzzle.Tests.Unit;

internal static class GlobalHooks
{
    public static string PluginsPath = "";

    [Before(TestSession)]
    public static void CopyPluginADll()
    {
        var assembly = typeof(ExportedMetadata).Assembly;
        CopyPluginDll(assembly);
    }

    internal static void CopyPluginDll(Assembly assembly)
    {
        var assemblyDir = new FileInfo(typeof(GlobalHooks).Assembly.Location).DirectoryName!;
        var pluginsDir = Path.Combine(assemblyDir, "plugins");
        var dllName = new FileInfo(assembly.Location).Name;
        var dll = Path.Combine(assemblyDir, dllName);
        var metadata = (IPluginMetadata)
            Activator.CreateInstance(
                assembly.ExportedTypes.First(t => t.IsAssignableTo(typeof(IPluginMetadata)))
            )!;
        var pluginDir = Path.Combine(pluginsDir, metadata.Id);

        if (Directory.Exists(pluginDir))
            Directory.Delete(pluginDir, true);

        Directory.CreateDirectory(pluginDir);
        File.Copy(dll, Path.Combine(pluginDir, dllName), true);
        PluginsPath = pluginsDir;
    }
}
