using System;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace PhoenixSagas.TCPServer.Implementations
{
    public class TcpServerHostedService : IHostedService
    {
        private readonly TcpNetworkServer _tcpServer;

        public TcpServerHostedService(TcpNetworkServer tcpServer)
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