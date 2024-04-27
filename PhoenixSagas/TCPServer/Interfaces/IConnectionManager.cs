using PhoenixSagas.TCPServer.Models;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace PhoenixSagas.TCPServer.Interfaes;

public interface IConnectionManager
{
    void HandleNewConnection(Socket socket);
    Task ReadClientInput(NetworkClient client, int clientId);
}