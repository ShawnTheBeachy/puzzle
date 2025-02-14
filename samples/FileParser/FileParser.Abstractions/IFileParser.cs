namespace FileParser.Abstractions;

public interface IFileParser
{
    bool TryParse(FileInfo fileInfo, out object? parsed);
}
