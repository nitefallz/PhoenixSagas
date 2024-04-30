using System.Threading;

namespace PhoenixSagas.TCPServer.Implementations;

public interface IOutputHandler
{
    public void Start(CancellationToken cancellationToken);
    public void ShutDown();
}