using FileParser.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Puzzle;

var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var services = new ServiceCollection().AddPlugins(configuration).BuildServiceProvider();

var path = args[0];
var fileInfo = new FileInfo(path);

var parsers = services.GetServices<IFileParser>().ToArray();

Console.WriteLine($"Trying {parsers.Length} parsers");

foreach (var parser in parsers)
{
    Console.WriteLine($"Trying {parser}");

    try
    {
        if (!parser.TryParse(fileInfo, out var parsed))
            continue;

        Console.WriteLine(parsed);
        break;
    }
    catch (Exception exception)
    {
        Console.WriteLine(exception);
        continue;
    }
}
