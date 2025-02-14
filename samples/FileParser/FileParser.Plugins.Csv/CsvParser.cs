using FileParser.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.FileIO;
using Puzzle.Abstractions;

namespace FileParser.Plugins.Csv;

[Service<IFileParser>(ServiceLifetime.Singleton)]
public sealed class CsvParser : IFileParser
{
    public bool TryParse(FileInfo fileInfo, out object? parsed)
    {
        parsed = null;

        if (fileInfo.Extension.ToUpperInvariant() != ".CSV")
            return false;

        using var parser = new TextFieldParser(fileInfo.FullName);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(",");

        var dataSet = new DataSet();

        while (!parser.EndOfData)
        {
            var row = dataSet.AddRow();

            var fields = parser.ReadFields();

            foreach (string field in fields ?? [])
                row.AddColumn(field);
        }

        parsed = dataSet;
        return true;
    }
}
