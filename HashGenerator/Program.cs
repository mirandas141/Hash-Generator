using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace HashGenerator
{
    [Command(Name = "HashGenerator", Description = "A simple utility to generate hashes from a file or directory of files")]
    [HelpOption("-?")]
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
            var app = new Application();
            await app.RunAsync(Target);
        }
    }
}
