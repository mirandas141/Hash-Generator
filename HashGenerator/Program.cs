using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Dynamic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace HashGenerator
{
    [Command(Name = "HashGenerator", Description = "A simple utility to generate hashes from a file or directory of files")]
    [HelpOption("-?|--help")]
    class Program
    {
        public static async Task Main(string[] args)
        {
            var services = ConfigureServices(new ServiceCollection())
                .BuildServiceProvider();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);

            await app.ExecuteAsync(args);
        }

        [Argument(0, nameof(Target), "The target file or directory to generate the hash(s) from")]
        public string Target { get; private set; }

        [Option(Description = "Used to specify the type of hash to generate" +
            "\nDefault: SHA256" +
            "\nValues: MD5 | SHA1 | SHA256 | SHA384 | SHA512" +
            "\nNote: MD5 and SHA1 are weaker hashing algorithms and not recommended.")]
        [AllowedValues("sha256", "sha384", "sha512", "md5", "sha1",
            IgnoreCase = true, 
            Comparer = StringComparison.InvariantCultureIgnoreCase, 
            ErrorMessage = "Supported types are ")]
        public string HashType { get; set; } = "sha256";

        private static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            return services.AddSingleton(x => new Application())
                .AddSingleton<IConsole>(PhysicalConsole.Singleton);
        }

        private async Task OnExecuteAsync(IConsole console)
        {
            while (string.IsNullOrWhiteSpace(Target))
            {
                Target = Prompt.GetString("Please enter the path: ").Trim();
            }

            HashAlgorithm hasher;
            switch (HashType.ToLowerInvariant().Trim())
            {
                case "sha256":
                default:
                    hasher = SHA256.Create();
                    break;
                case "sha384":
                    hasher = SHA384.Create();
                    break;
                case "sha512":
                    hasher = SHA512.Create();
                    break;
                case "md5":
                    hasher = MD5.Create();
                    break;
                case "sha1":
                    hasher = SHA1.Create();
                    break;
                    
            }

            var app = new Application();
            await app.RunAsync(Target, hasher);
        }
    }
}
