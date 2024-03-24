using System.Threading.Tasks;
using System.Threading;

namespace PhoenixSagas.TCPServer.Interfaes
{
    public interface ITcpNetworkServer
    {
        public Task Start(CancellationToken cancellationToken);
        public void ShutDown();
    }
}