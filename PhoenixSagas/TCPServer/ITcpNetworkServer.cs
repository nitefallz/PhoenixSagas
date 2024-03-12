using System.Threading.Tasks;
using System.Threading;

namespace PhoenixSagas.TcpServer.Implementations
{
    public interface ITcpNetworkServer
    {
        public Task StartAsync(CancellationToken cancellationToken);
        public void ShutDown();
    }
}