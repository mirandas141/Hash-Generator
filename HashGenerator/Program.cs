using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Dynamic;
using System.Security.Cryptography;
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

        [Argument(0, nameof(Source), "The target file or directory to generate the hash(s) from")]
        public string Source { get; private set; }

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
            return services.AddSingleton<IConsole>(PhysicalConsole.Singleton)
                .AddScoped<IHashGenerator, HashGenerator>();
        }

        private async Task OnExecuteAsync()
        {
            while (string.IsNullOrWhiteSpace(Source))
            {
                Source = Prompt.GetString("Please enter the path: ").Trim();
            }

            using HashAlgorithm hasher = (HashType.ToLowerInvariant().Trim()) switch
            {
                "sha384" => SHA384.Create(),
                "sha512" => SHA512.Create(),
                "md5" => MD5.Create(),
                "sha1" => SHA1.Create(),
                _ => SHA256.Create(),
            };

            var app = new Application();
            await app.RunAsync(Source, hasher);
        }
    }
}
