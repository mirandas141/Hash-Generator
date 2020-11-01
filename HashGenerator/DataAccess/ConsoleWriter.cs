using McMaster.Extensions.CommandLineUtils;
using System.Threading.Tasks;

namespace HashGenerator.DataAccess
{
    public class ConsoleWriter : IOutputTextWriter
    {
        private readonly IConsole _console;

        public ConsoleWriter(IConsole console)
        {
            _console = console;
        }

        public async Task Write(string output)
        {
            _console.Write(output);
        }
    }
}
