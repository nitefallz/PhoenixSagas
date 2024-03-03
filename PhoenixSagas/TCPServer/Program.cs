using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoenixSagas.TCPSocketServer.Implementations;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace PhoenixSagas.TCPServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    // Include additional configuration files as needed:
                    // config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<TcpNetworkServer>();
                    services.AddHostedService<TcpServerHostedService>();
                    // Add any additional services or configurations here
                });
    }
}