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
            WriteMessgage(ex.Message);
            return (int)ExitCode.SourceNotFound;
        }
        catch (Exception ex) when 
            (ex is CommandParsingException 
            || ex is NoOutputEnabledException)
        {
            WriteMessgage(ex.Message);
            return (int)ExitCode.InvalidParameter;
        }
        catch (Exception ex)
        {
            WriteMessgage(ex.Message);
            return (int)ExitCode.UndefinedError;
        }
    }

    private static void WriteMessgage(string message) => Console.WriteLine(message);

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(PhysicalConsole.Singleton);
    }
}
