using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashGenerator
{
    public class Application
    {
        public async Task RunAsync(string target, HashAlgorithm hasher)
        {
            Console.WriteLine($"Run Async {target}");
        }
    }
}
