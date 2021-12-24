namespace HashGenerator;

public interface IHashGenerator
{
    Task<string> FromFileAsync(string target);
}
