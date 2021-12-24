namespace HashGenerator;

public struct HashPair
{
    public HashPair(string name, string hash)
    {
        Name = name;
        Hash = hash;
    }

    public string Name { get; }
    public string Hash { get; }
}
