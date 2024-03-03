using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using PhoenixSagas.TCPSocketServer.Implementations;

public class TcpServerHostedService : IHostedService
{
    private readonly TcpNetworkServer _tcpNetworkServer;

    public TcpServerHostedService(TcpNetworkServer tcpNetworkServer)
    {
        _tcpNetworkServer = tcpNetworkServer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _tcpNetworkServer.Start();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _tcpNetworkServer.ShutDown();
        return Task.CompletedTask;
    }
}