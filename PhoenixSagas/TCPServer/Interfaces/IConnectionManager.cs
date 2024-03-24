using System.Net.Sockets;
using System.Threading.Tasks;
using PhoenixSagas.TcpServer.Implementations;
using PhoenixSagas.TCPServer.Models;

namespace PhoenixSagas.TCPServer.Interfaes;

public interface IConnectionManager
{
    void HandleNewConnection(Socket socket);
    Task ReadClientInput(NetworkClient client, int clientId);
}