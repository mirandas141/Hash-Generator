using HashGenerator.DataAccess;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace HashGenerator
{
    [Command(Name = "HashGenerator", 
        FullName = "Hash Generator",
        Description = "A simple utility to generate hashes from a file or directory of files")]
    [VersionOptionFromMember(MemberName = "GetVersion")]
    [HelpOption]
    public class Startup
    {
        private readonly IConsole _console;

        public Startup(IConsole console)
        {
            _console = console;
        }

        private async Task OnExecuteAsync()
        {
            while (string.IsNullOrWhiteSpace(Source))
            {
                Source = Prompt.GetString("Please enter the path: ").Trim();
            }

            var hasher = GetHashAlgorithm(HashType);
            var generator = new HashGenerator(hasher)
            {
                RelativePaths = !FullPaths
            };

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

            if (PauseOnCompletion)
            {
                _console.Write("Press any key to continue: ");
                var reader = _console.In;
                reader.Read();
            }
        }

        private HashAlgorithm GetHashAlgorithm(string hashType)
        {
            return (hashType.ToLowerInvariant().Trim()) switch
            {
                "sha384" => SHA384.Create(),
                "sha512" => SHA512.Create(),
                "md5" => MD5.Create(),
                "sha1" => SHA1.Create(),
                _ => SHA256.Create()
            };
        }

        private string GetVersion()
            => typeof(Startup)
            .Assembly?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        [Argument(0, nameof(Source), "The target file or directory to generate the hash(s) from")]
        public string Source { get; set; }

        [Option(Description = "Used to specify the algorithm to use in generating the hash\n" +
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

        [Option(Description = "Output files with full paths")]
        public bool FullPaths { get; set; }

        [Option(Description = "Does not output results to the console")]
        public bool Silent { get; set; }

        [Option(Description = "Pause on completion. Useful if running from a shortcut.")]
        public bool PauseOnCompletion { get; set; }
    }
}
