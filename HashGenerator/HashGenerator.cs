using System;
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
        private string _pathToTrim = "";

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

        public HashGenerator UseRelativePaths(bool useRelative)
        {
            RelativePaths = useRelative;
            return this;
        }

        public bool RelativePaths { get; set; }

        public async Task<List<HashPair>> FromDirectoryAsync(string directory, string pattern = "*")
        {
            SetPathToTrim(directory);

            var result = new List<HashPair>();
            result.AddRange(await ComputeFromDirecotryRecursiveAsync(new DirectoryInfo(directory), pattern));

            if (result.Count > 1)
                result.Add(CreateCombinedHash(result));

            return result;
        }

        private async Task<List<HashPair>> ComputeFromDirecotryRecursiveAsync(DirectoryInfo directory, string pattern = "*")
        {
            var files = directory.GetFiles(pattern).OrderBy(x => x.FullName);
            var result = new List<HashPair>();

            foreach (var file in files)
            {
                result.Add(await ComputerFromFileAsync(file.FullName));
            }

            foreach (var dir in directory.GetDirectories())
            {
                result.AddRange(await ComputeFromDirecotryRecursiveAsync(dir, pattern));
            }

            return result;
        }

        private async Task<HashPair> ComputerFromFileAsync(string file)
        {
            var bytes = await File.ReadAllBytesAsync(file);
            var hash = _hasher.ComputeHash(bytes);
            var result = HashToString(hash);
            file = file.Replace(_pathToTrim, "");
            return new HashPair(file, result);
        }

        public async Task<HashPair> FromFileAsync(string file)
        {
            SetPathToTrim(file);

            return await ComputerFromFileAsync(file);
        }

        private void SetPathToTrim(string file)
        {
            if (RelativePaths)
                _pathToTrim = Directory.GetParent(file).FullName + Path.DirectorySeparatorChar;
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
