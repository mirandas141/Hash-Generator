using HashGenerator.DataAccess;
using HashGenerator.Models;
using McMaster.Extensions.CommandLineUtils;
using System.Reflection;

namespace HashGenerator;

[Command(Name = "HashGenerator",
    FullName = "Hash Generator",
    Description = "A simple utility to generate hashes from a file or directory of files")]
[VersionOptionFromMember(MemberName = "GetVersion")]
[HelpOption(ShortName = "")] //Hide the default -h parameter as it conflicts with HashType (-h) below
public class Startup
{
    private readonly IConsole _console;

    public Startup(IConsole console)
    {
        _console = console;
    }

    internal async Task OnExecuteAsync()
    {
        if (Silent && string.IsNullOrWhiteSpace(Target))
            throw new NoOutputEnabledException();

        while (string.IsNullOrWhiteSpace(Source))
        {
            Source = Prompt.GetString("Please enter the path: ").Trim();
        }

        var generator = HashGenerator.Create(HashType);
        var output = new TextOutput(GetWritters(Source), GetFormatter(Source));
        var app = new Application(generator, output);
        await app.RunAsync(Source);

        if (PauseOnCompletion)
        {
            _console.Write("Press any key to continue: ");
            var reader = _console.In;
            reader.Read();
        }
    }

    private List<IOutputTextWriter> GetWritters(string source)
    {
        var writers = new List<IOutputTextWriter>();
        if (!Silent)
        {
            writers.Add(new ConsoleWriter(_console));
        }
        if (!string.IsNullOrWhiteSpace(Target))
        {
            if (Target.Equals(".", StringComparison.InvariantCultureIgnoreCase))
            {
                Target = $"{source}.{HashType}";
            }
            var file = new FileWriter(Target);
            writers.Add(file);
        }

        return writers;
    }

    private INameFormatter GetFormatter(string source)
    {
        return FullPaths
            ? new FullPathNameFormatter()
            : new RelativePathNameFormatter(source);
    }

    internal string GetVersion()
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
        Comparer = StringComparison.InvariantCultureIgnoreCase
        /*ErrorMessage = "Supported types are "*/)]
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
