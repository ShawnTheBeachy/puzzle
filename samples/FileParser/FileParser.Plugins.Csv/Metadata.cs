using Puzzle.Abstractions;

namespace FileParser.Plugins.Csv;

public sealed class Metadata : IPluginMetadata
{
    public string Id => "com.fileparser.csv";
    public string Name => "CSV Parser";
}
