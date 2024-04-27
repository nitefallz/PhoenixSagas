using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace PhoenixSagas.GameServer;

public class GameServerHostedService : IHostedService
{
    private readonly IGameServer _gameServer;
    public GameServerHostedService(IGameServer gameServer)
    {
        _gameServer = gameServer;
    }
    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _gameServer.Start(cancellationToken);
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
            _gameServer.ShutDown();
            return Task.CompletedTask;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}