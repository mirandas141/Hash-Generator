namespace HashGenerator;

public class Application
{
    private readonly IHashGenerator _hashGenerator;
    private readonly IOutput _output;

    internal Application(IHashGenerator hashGenerator, IOutput output)
    {
        _hashGenerator = hashGenerator;
        _output = output;
    }

    public async Task RunAsync(string source)
    {
        var files = await LocatesFiles(source);
        if (files.Count == 0) ThrowSourceNotFoundException(source);

        await ComputeAndWriteHashes(files);
    }

    private static void ThrowSourceNotFoundException(string source)
    {
        throw new SourceNotFoundException(source);
    }

    private async Task<List<string>> LocatesFiles(string source)
    {
        var files = new List<string>();
        if (File.Exists(source))
        {
            files.Add(source);
        }
        else if (Directory.Exists(source))
        {
            files.AddRange(Directory.GetFiles(source));
            foreach (var dir in Directory.GetDirectories(source))
            {
                files.AddRange(await LocatesFiles(dir));
            }
        }

        return files;
    }

    private async Task ComputeAndWriteHashes(List<string> files)
    {
        foreach (var file in files)
        {
            var hash = await _hashGenerator.FromFileAsync(file);
            await OutputResult(new HashPair(file, hash));
        }
    }

    private async Task OutputResult(HashPair pair)
    {
        await _output.Write(pair);
    }
}
