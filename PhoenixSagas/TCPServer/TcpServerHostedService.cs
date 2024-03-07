using Microsoft.Extensions.Hosting;
using PhoenixSagas.TcpServer.Implementations;
using System.Threading;
using System.Threading.Tasks;

public class TcpServerHostedService : IHostedService
{
    private readonly TcpSerer _tcpSerer;

    public TcpServerHostedService(TcpSerer tcpSerer)
    {
        _tcpSerer = tcpSerer;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _tcpSerer.StartAsync(cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _tcpSerer.ShutDown();
        return Task.CompletedTask;
    }
}