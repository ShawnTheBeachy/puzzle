using Microsoft.AspNetCore.Components;

namespace FileParser.Abstractions;

public interface IFileParser
{
    RenderFragment? Display(object parsed);
    bool TryParse(FileInfo fileInfo, Stream data, out object? parsed);
}
