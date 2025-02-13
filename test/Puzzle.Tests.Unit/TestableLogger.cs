using Microsoft.Extensions.Logging;

namespace Puzzle.Tests.Unit;

internal sealed class TestableLogger<T> : ILogger<T>
{
    private readonly List<LogMessage> _logMessages = [];
    public IReadOnlyList<LogMessage> LogMessages => _logMessages;

    public IDisposable? BeginScope<TState>(TState state)
        where TState : notnull => Substitute.For<IDisposable>();

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        var message = formatter(state, exception);
        var logMessage = new LogMessage(logLevel, eventId, exception, message);
        _logMessages.Add(logMessage);
    }

    public sealed record LogMessage(
        LogLevel LogLevel,
        EventId EventId,
        Exception? Exception,
        string Message
    );
}
