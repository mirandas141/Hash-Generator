namespace HashGenerator;

public interface IOutput
{
    Task Write(List<HashPair> hashes);
}
