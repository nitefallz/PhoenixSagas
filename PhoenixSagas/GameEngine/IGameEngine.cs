namespace PhoenixSagas.Interfaces.GameEngine;

public interface IGameEngine
{
    public Task Start(CancellationToken cancellationToken);
    public void ShutDown();
}