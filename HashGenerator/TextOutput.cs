using HashGenerator.DataAccess;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace HashGenerator
{
    public class TextOutput : IOutput
    {
        private readonly List<IOutputTextWriter> _writers = new List<IOutputTextWriter>();

        public TextOutput(IOutputTextWriter writer)
        {
            _writers.Add(writer);
        }

        public TextOutput(List<IOutputTextWriter> writers)
        {
            _writers.AddRange(writers);
        }

        public async Task Write(List<HashPair> hashes)
        {
            var output = new StringBuilder();

            foreach (var hash in hashes)
            {
                output.AppendLine($"{hash.Name} => {hash.Hash}");
            }

            foreach (var writer in _writers)
            {
                await writer.Write(output.ToString());
            }
        }
    }
}
