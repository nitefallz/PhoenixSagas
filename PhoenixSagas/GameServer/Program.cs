using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhoenixSagas.Kafka.Implementations;
using PhoenixSagas.Kafka.Interfaces;
using System.IO;


namespace PhoenixSagas.GameServer
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
                    services.AddHostedService<GameServerHostedService>();
                    services.AddSingleton<IKafkaFactory, KafkaFactory>();
                    services.AddSingleton<IGameEngine, GameEngine>();
                    services.AddSingleton<IGameServer, GameServer>();
                    // Add any additional services or configurations here
                });
    }
}