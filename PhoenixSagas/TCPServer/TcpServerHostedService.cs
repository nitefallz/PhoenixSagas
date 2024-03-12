using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace PhoenixSagas.TcpServer.Implementations
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
            _tcpServer.StartAsync(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tcpServer.ShutDown();
            return Task.CompletedTask;
        }
    }
}