using System.Security.Cryptography;

namespace HashGenerator;

public class HashGenerator : IHashGenerator
{
    private readonly HashAlgorithm _hasher;

    private HashGenerator(HashAlgorithm hasher)
    {
        _hasher = hasher;
    }

    public static HashGenerator Create(string hashType)
    {
        if (string.IsNullOrEmpty(hashType)) throw new ArgumentNullException(nameof(hashType));

        var crypto = HashAlgorithm.Create(hashType);
        return new HashGenerator(crypto);
    }

    public async Task<string> FromFileAsync(string file)
    {
        var bytes = await File.ReadAllBytesAsync(file);
        var hash = _hasher.ComputeHash(bytes);
        return HashToString(hash);
    }

    private string HashToString(byte[] hash)
    {
        var result = new StringBuilder();

        foreach (var b in hash)
        {
            result.Append(b.ToString("x2").ToUpper());
        }

        return result.ToString();
    }
}
