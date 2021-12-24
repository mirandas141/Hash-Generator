namespace HashGenerator.DataAccess;

public class FileWriter : IOutputTextWriter
{
    private readonly string _target;

    public FileWriter(string target)
    {
        _target = target;
    }

    public async Task Write(string output)
    {
        var parent = Directory.GetParent(_target);
        if (!parent.Exists)
            parent.Create();

        await File.WriteAllTextAsync(_target, output);
    }
}
