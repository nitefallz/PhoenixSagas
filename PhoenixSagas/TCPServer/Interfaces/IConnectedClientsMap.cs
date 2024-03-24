using System.Collections.Concurrent;
using PhoenixSagas.TCPServer.Models;

namespace PhoenixSagas.TCPServer.Interfaes;

public interface IConnectedClientsMap
{
    ConcurrentDictionary<int, NetworkClient> Map { get; set; }
    NetworkClient GetClient(int id);
}