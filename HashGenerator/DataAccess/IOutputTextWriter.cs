namespace HashGenerator.DataAccess;

public interface IOutputTextWriter
{
    Task Write(string output);
}
