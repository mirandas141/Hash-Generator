namespace HashGenerator.DataAccess;

public interface IOutputTextWriter
{
    Task WriteLineAsync(string output);
}
