using McMaster.Extensions.CommandLineUtils;

namespace HashGenerator.DataAccess
{
    public class ConsoleWriter : IOutputTextWriter
    {
        private readonly IConsole _console;

        public ConsoleWriter(IConsole console)
        {
            _console = console;
        }

        public Task Write(string output)
        {
            _console.Write(output);
            return Task.CompletedTask;
        }
    }
}
