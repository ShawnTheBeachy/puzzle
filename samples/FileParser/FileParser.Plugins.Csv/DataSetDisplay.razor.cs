using Microsoft.AspNetCore.Components;

namespace FileParser.Plugins.Csv;

public sealed partial class DataSetDisplay
{
    [Parameter, EditorRequired]
    public required DataSet DataSet { get; set; }
}
