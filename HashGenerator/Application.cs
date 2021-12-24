namespace HashGenerator;

public class Application
{
    private readonly IHashGenerator _hashGenerator;
    private readonly IOutput _output;

    public Application(IHashGenerator hashGenerator, IOutput output)
    {
        _hashGenerator = hashGenerator;
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
        else if (source.Contains("*") || source.Contains("?"))
        {
            if (source.Contains(Path.DirectorySeparatorChar))
            {
                var dir = Path.GetDirectoryName(source);
                var pattern = source
                    .Replace(dir, string.Empty)
                    .Replace(Path.DirectorySeparatorChar.ToString(), string.Empty);
                hashes.AddRange(await _hashGenerator.FromDirectoryAsync(dir, pattern));
            }
            else
            {
                hashes.AddRange(await _hashGenerator.FromDirectoryAsync(Directory.GetCurrentDirectory(), source));
            }
        }
        else
        {
            throw new SourceNotFoundException(source);
        }
        return hashes;
    }

    private void OutputResults(List<HashPair> hashes)
    {
        _output.Write(hashes);
    }
}
