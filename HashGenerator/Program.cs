using HashGenerator.DataAccess;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace HashGenerator
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .RunCommandLineApplicationAsync<Startup>(args);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(PhysicalConsole.Singleton);
        }
    }
}
