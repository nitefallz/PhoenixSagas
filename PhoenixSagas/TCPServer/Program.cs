using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoenixSagas.Kafka.Implementations;
using PhoenixSagas.Kafka.Interfaces;
using System.IO;
using PhoenixSagas.TCPServer.Interfaes;
using PhoenixSagas.TCPServer.Implementations;

namespace PhoenixSagas.TcpServer
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
                    services.AddSingleton<ITcpNetworkServer,TcpNetworkServer>();
                    services.AddHostedService<TcpServerHostedService>();
                    services.AddSingleton<IConnectedClientsMap, ConnectedClientMap>();
                    services.AddSingleton<IConnectionManager, ConnectionManager>();
                    services.AddSingleton<IKafkaFactory, KafkaFactory>();
                    // Add any additional services or configurations here
                });
    }
}