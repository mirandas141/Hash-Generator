using HashGenerator.DataAccess;
using HashGenerator.Models;

namespace HashGenerator;

internal class TextOutput : IOutput
{
    private readonly List<IOutputTextWriter> _writers = new List<IOutputTextWriter>();
    private readonly INameFormatter _nameFormatter;

    internal TextOutput(IOutputTextWriter writer, INameFormatter nameFormatter)
    {
        _writers.Add(writer);
        _nameFormatter = nameFormatter;
    }

    public TextOutput(List<IOutputTextWriter> writers, INameFormatter nameFormatter)
    {
        _writers.AddRange(writers);
        _nameFormatter = nameFormatter;
    }

    public async Task Write(HashPair pair)
    {
        foreach (var writer in _writers)
        {
            var name = _nameFormatter.Format(pair.Name);
            await writer.WriteLineAsync($"{name} => {pair.Hash}");
        }
    }
}
