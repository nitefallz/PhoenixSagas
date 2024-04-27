namespace PhoenixSagas.GameServer;

public interface IGameServer
{
    public void Start(CancellationToken cancellationToken);
    public void ShutDown();

}