using HashGenerator.DataAccess;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HashGenerator
{
    [Command(Name = "HashGenerator", Description = "A simple utility to generate hashes from a file or directory of files")]
    [HelpOption("-?|--help")]
    class Program
    {
        private static IServiceProvider _serviceProvider;
        private readonly IConsole _console;

        public static async Task Main(string[] args)
        {
            _serviceProvider = ConfigureServices();
                //.BuildServiceProvider();

            var app = new CommandLineApplication<Program>();
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(_serviceProvider);

            await app.ExecuteAsync(args);
        }

        //private ServiceProvider _provider { get; set; }

        [Argument(0, nameof(Source), "The target file or directory to generate the hash(s) from")]
        public string Source { get; private set; }

        [Option(Description = "Used to specify the type of hash to generate\n" +
            "Default: SHA256\n" +
            "Values: MD5 | SHA1 | SHA256 | SHA384 | SHA512\n" +
            "Note: MD5 and SHA1 are weaker hashing algorithms and not recommended.")]
        [AllowedValues("sha256", "sha384", "sha512", "md5", "sha1",
            IgnoreCase = true, 
            Comparer = StringComparison.InvariantCultureIgnoreCase, 
            ErrorMessage = "Supported types are ")]
        public string HashType { get; set; } = "sha256";

        [Option(Description = "Target file to write hashes too\n" +
            "specifying '.' will result in using the specified [file|directory].hashType as file name")]
        public string Target { get; set; }

        [Option(Description = "Output files as relative paths")]
        public bool RelativePaths { get; set; }

        [Option(Description = "Does not output results to the console")]
        public bool Silent { get; set; }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton(PhysicalConsole.Singleton);
            services.AddScoped<IHashGenerator, HashGenerator>();
            services.AddScoped<IOutput, TextOutput>();
            services.AddTransient<FileWriter>();
            services.AddTransient<ConsoleWriter>();
            return services.BuildServiceProvider();
        }

        public Program(IConsole console)
        {
            _console = console;
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
                _ => SHA256.Create()
            };

            var generator = new HashGenerator(hasher);
            generator.RelativePaths = RelativePaths;

            var writers = new List<IOutputTextWriter>();
            if (!Silent)
            {
                writers.Add(new ConsoleWriter(_console));
            }
            if (!string.IsNullOrWhiteSpace(Target))
            {
                if (Target.Equals(".", StringComparison.InvariantCultureIgnoreCase))
                {
                    Target = $"{Source}.{HashType}";
                }
                var file = new FileWriter(Target);
                writers.Add(file);
            }

            IOutput output = new TextOutput(writers);
            var app = new Application(generator, _console, output);
            await app.RunAsync(Source);
        }
    }
}
