﻿namespace HashGenerator.DataAccess;

public class FileWriter : IOutputTextWriter
{
    private readonly string _target;

    public FileWriter(string target)
    {
        _target = target;
    }

    public async Task WriteLineAsync(string output)
    {
        var parent = Directory.GetParent(_target);
        if (!parent.Exists)
            parent.Create();

        if (!output.EndsWith(Environment.NewLine))
            output += Environment.NewLine;

        await File.AppendAllTextAsync(_target, output);
    }
}
