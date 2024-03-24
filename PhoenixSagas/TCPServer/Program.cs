using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoenixSagas.Kafka.Implementations;
using PhoenixSagas.Kafka.Interfaces;
using PhoenixSagas.Models;
using PhoenixSagas.TCPServer;
using PhoenixSagas.TCPServer.Implementations;
using PhoenixSagas.TCPServer.Interfaces;
using System.IO;

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
                    services.AddSingleton<IMessageHandler<PlayerOutput>, OutputHandler>();


                    // Add any additional services or configurations here
                });
    }
}