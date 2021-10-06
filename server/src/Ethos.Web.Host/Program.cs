using System.Threading.Tasks;
using Ethos.Application.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Ethos.Web.Host
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var hostBuilder = CreateHostBuilder(args).Build();

            // seed database
            using var scope = hostBuilder.Services.CreateScope();
            foreach (var dataSeedContributor in scope.ServiceProvider.GetServices<IDataSeedContributor>())
            {
                await dataSeedContributor.SeedAsync();
            }

            await hostBuilder.RunAsync();
        }

        /// <summary>
        /// Creates the host builder reading environment variables.
        /// </summary>
        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host
                .CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "Ethos_");
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseUrls("http://0.0.0.0:5001");
                });
    }
}
