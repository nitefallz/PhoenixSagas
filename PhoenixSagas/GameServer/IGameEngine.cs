namespace PhoenixSagas.GameServer;

public interface IGameEngine
{
    public void Start(CancellationToken cancellationToken);
    public void ShutDown();
}