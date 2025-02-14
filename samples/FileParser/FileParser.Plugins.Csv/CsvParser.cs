using FileParser.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using Puzzle.Abstractions;

namespace FileParser.Plugins.Csv;

[Service<IFileParser>(ServiceLifetime.Singleton)]
public sealed class CsvParser : IFileParser
{
    private readonly ILogger _logger;

    public CsvParser(ILogger<CsvParser> logger)
    {
        _logger = logger;
    }

    public RenderFragment? Display(object parsed)
    {
        if (parsed is not DataSet dataSet)
            return null;

        return b =>
        {
            b.OpenComponent<DataSetDisplay>(0);
            b.AddComponentParameter(1, nameof(DataSetDisplay.DataSet), dataSet);
            b.CloseComponent();
        };
    }

    public bool TryParse(FileInfo fileInfo, Stream data, out object? parsed)
    {
        parsed = null;
        var extension = fileInfo.Extension.ToUpperInvariant();

        if (extension is not (".CSV" or ".TSV"))
            return false;

        using var parser = new TextFieldParser(data);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(extension == ".TSV" ? "\t" : ",");

        var dataSet = new DataSet();

        while (!parser.EndOfData)
        {
            var row = dataSet.AddRow();

            var fields = parser.ReadFields();

            foreach (string field in fields ?? [])
                row.AddColumn(field);
        }

        _logger.LogInformation("Parsed {Count} rows", dataSet.Rows.Count);
        parsed = dataSet;
        return true;
    }
}
