using System.Linq;
using System.Threading.Tasks;
using Ethos.Application.Seed;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Ethos.Web.Host
{
    public static class Program
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
                    config.AddEnvironmentVariables();
                })
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Filter.ByExcluding(logEvent => logEvent
                        .Properties
                        .Any(p =>
                            p.Key == "RequestPath" && (
                            p.Value.ToString().Contains("/node_modules") ||
                            p.Value.ToString().Contains("/assets") ||
                            p.Value.ToString().Contains("/svg") ||
                            p.Value.ToString().Contains(".js") ||
                            p.Value.ToString().Contains(".css") ||
                            p.Value.ToString().Contains(".ttf") ||
                            p.Value.ToString().Contains(".jpg") ||
                            p.Value.ToString().Contains(".png") ||
                            p.Value.ToString().Contains(".map")))))
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
