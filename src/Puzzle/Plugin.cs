using System.Diagnostics;
using System.Reflection;
using Puzzle.Abstractions;

namespace Puzzle;

public sealed class Plugin
{
    public ITypeProvider AllTypes { get; }
    public Assembly Assembly { get; }
    public Type? BootstrapperType { get; }
    public string Id { get; }
    public string Name { get; }
    public string? Version { get; }

    internal Plugin(
        ITypeProvider allTypes,
        Assembly assembly,
        Type? bootstrapperType,
        IPluginMetadata metadata
    )
    {
        AllTypes = allTypes;
        Assembly = assembly;
        BootstrapperType = bootstrapperType;
        Id = metadata.Id;
        Name = metadata.Name;
        Version = FileVersionInfo.GetVersionInfo(assembly.Location).ProductVersion;
    }

    internal static bool TryCreate(Assembly assembly, out Plugin? plugin)
    {
        plugin = null;

        try
        {
            var exportedTypes = assembly.ExportedTypes.ToArray();
            var metadataType = exportedTypes.FirstOrDefault(
                typeof(IPluginMetadata).IsAssignableFrom
            );

            if (metadataType is null)
                return false;

            plugin = new Plugin(
                new TypeProvider(exportedTypes),
                assembly,
                exportedTypes.FirstOrDefault(typeof(IPluginBootstrapper).IsAssignableFrom),
                (IPluginMetadata)Activator.CreateInstance(metadataType)!
            );
            return true;
        }
        catch
        {
            return false;
        }
    }

    private sealed class TypeProvider : ITypeProvider
    {
        private readonly IReadOnlyList<Type> _types;

        public TypeProvider(IReadOnlyList<Type> types)
        {
            _types = types;
        }

        public IEnumerable<Type> GetTypes() => _types;
    }
}
