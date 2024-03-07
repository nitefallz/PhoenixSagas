using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoenixSagas.TcpServer.Implementations;
using PhoenixSagas.TCPServer.Implementations;
using System.IO;
using PhoenixSagas.Kafka.Implementations;
using PhoenixSagas.Kafka.Interfaces;
using PhoenixSagas.Models;

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
                    services.AddSingleton<TcpSerer>();
                    services.AddHostedService<TcpServerHostedService>();
                    services.AddSingleton<IConnectedClientsMap, ConnectedClientMap>();
                    services.AddSingleton<ConnectionManager>();
                    services.AddSingleton<IKafkaFactory, KafkaFactory>();
                    // Add any additional services or configurations here
                });
    }
}