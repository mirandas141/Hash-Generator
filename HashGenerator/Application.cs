using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HashGenerator
{
    public class Application
    {
        private readonly IHashGenerator _hashGenerator;
        private readonly IConsole _console;
        private readonly IOutput _output;

        public Application(IHashGenerator hashGenerator, IConsole console, IOutput output)
        {
            _hashGenerator = hashGenerator;
            _console = console;
            _output = output;
        }

        public async Task RunAsync(string source)
        {
            var hashes = await ComputeHashes(source);

            OutputResults(hashes);
        }

        private async Task<List<HashPair>> ComputeHashes(string source)
        {
            var hashes = new List<HashPair>();
            source = source.Replace("\"", "").Replace("\'", "");

            if (File.Exists(source))
            {
                hashes.Add(await _hashGenerator.FromFileAsync(source));
            }
            else if (Directory.Exists(source))
            {
                hashes.AddRange(await _hashGenerator.FromDirectoryAsync(source));
            }
            else
            {
                _console.WriteLine("Source not found");
                Environment.Exit(-1);
            }
            return hashes;
        }

        private void OutputResults(List<HashPair> hashes)
        {
            _output.Write(hashes);
        }
    }
}
