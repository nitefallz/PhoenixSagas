//using ForgottenRealms.TCPSocketServer.Implementations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace PhoenixSagas.TCPServer
{
    class TcpNetwork
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());
                //.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            var service = new ServiceCollection()
                .AddLogging()
                .AddSingleton<TcpNetworkServer, TcpNetworkServer>()
                //.AddSingleton<IConnectedClientsMap, ConnectedClientMap>()
                //.AddSingleton<IKafkaFactory, KafkaFactory>()
                .BuildServiceProvider();

            var tcpServer = service.GetService<TcpNetworkServer>();
            tcpServer.Start();

        }
    }
}