using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;

namespace HashGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = BuildConfiguration();

            var host = CreateDefaultHost();

            var app = host.Services.GetService<Application>();

            app.Run();
        }

        private static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private static IHost CreateDefaultHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped(x => new Application());
        }
    }
}
