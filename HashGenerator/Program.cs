using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HashGenerator;

class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            await Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .RunCommandLineApplicationAsync<Startup>(args);
            return (int)ExitCode.Ok;
        }
        catch (SourceNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
            return (int)ExitCode.SourceNotFound;
        }
        catch (CommandParsingException ex)
        {
            return (int)ExitCode.InvalidParameter;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.GetType());
            return (int)ExitCode.UndefinedError;
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(PhysicalConsole.Singleton);
    }
}
