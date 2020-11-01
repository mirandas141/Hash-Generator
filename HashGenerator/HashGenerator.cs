using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashGenerator
{
    public class HashGenerator : IHashGenerator
    {
        private readonly HashAlgorithm _hasher;

        public HashGenerator(HashAlgorithm hasher)
        {
            _hasher = hasher;
        }

        public async Task<List<HashPair>> FromDirectoryAsync(string directory)
        {
            var result = new List<HashPair>();
            result.AddRange(await ComputeFromDirecotryRecursiveAsync(directory));
            result.Add(CreateCombinedHash(result));

            return result;
        }

        private async Task<List<HashPair>> ComputeFromDirecotryRecursiveAsync(string directory)
        {
            return await ComputeFromDirecotryRecursiveAsync(new DirectoryInfo(directory));
        }

        private async Task<List<HashPair>> ComputeFromDirecotryRecursiveAsync(DirectoryInfo directory)
        {
            var files = directory.GetFiles().OrderBy(x => x.FullName);
            var result = new List<HashPair>();

            foreach (var file in files)
            {
                result.Add(await FromFileAsync(file.FullName));
            }

            foreach (var dir in directory.GetDirectories())
            {
                result.AddRange(await ComputeFromDirecotryRecursiveAsync(dir));
            }

            return result;
        }

        public async Task<HashPair> FromFileAsync(string file)
        {
            var bytes = await File.ReadAllBytesAsync(file);
            var hash = _hasher.ComputeHash(bytes);
            var result = HashToString(hash);
            return new HashPair(file, result);
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

        public HashPair CreateCombinedHash(List<HashPair> hashPairs)
        {
            var combined = string.Join("", hashPairs.Select(x => x.Hash));
            var bytes = Encoding.UTF8.GetBytes(combined);
            var hash = _hasher.ComputeHash(bytes);
            return new HashPair("Combined", HashToString(hash));
        }
    }
}
