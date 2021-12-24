using McMaster.Extensions.CommandLineUtils;

namespace HashGenerator.DataAccess;

public class ConsoleWriter : IOutputTextWriter
{
    private readonly IConsole _console;

    public ConsoleWriter(IConsole console)
    {
        _console = console;
    }

    public Task WriteLineAsync(string output)
    {
        _console.WriteLine(output);
        return Task.CompletedTask;
    }
}
