namespace FileParser.Plugins.Csv;

internal sealed class DataSet
{
    private readonly List<Row> _rows = [];

    public Row AddRow()
    {
        var row = new Row(this);
        _rows.Add(row);
        return row;
    }

    public int GetWidth(int index)
    {
        var width = 0;

        foreach (var row in _rows)
        {
            var rowWidth = row.GetWidth(index);

            if (rowWidth > width)
                width = rowWidth;
        }

        return width;
    }

    public override string ToString() => string.Join('\n', _rows);
}

internal sealed class Row
{
    private readonly List<string> _columns = [];
    private readonly DataSet _dataSet;

    public Row(DataSet dataSet)
    {
        _dataSet = dataSet;
    }

    public void AddColumn(string value) => _columns.Add(value);

    public int GetWidth(int index) => _columns[index].Length;

    public override string ToString() =>
        string.Join('\t', _columns.Select((col, i) => col.PadRight(_dataSet.GetWidth(i))));
}
