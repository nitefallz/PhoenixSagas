using System;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using PhoenixSagas.TCPServer.Interfaes;

namespace PhoenixSagas.TCPServer.Implementations
{
    public class TcpServerHostedService : IHostedService
    {
        private readonly ITcpNetworkServer _tcpServer;

        public TcpServerHostedService(ITcpNetworkServer tcpServer)
        {
            _tcpServer = tcpServer;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                _tcpServer.Start(cancellationToken);
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            try
            {
                _tcpServer.ShutDown();
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}