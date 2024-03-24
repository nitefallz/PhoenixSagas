using System.Collections.Concurrent;
using PhoenixSagas.TCPServer.Interfaes;
using PhoenixSagas.TCPServer.Models;

namespace PhoenixSagas.TCPServer.Implementations;

public class ConnectedClientMap : IConnectedClientsMap
{
    public ConcurrentDictionary<int, NetworkClient> Map { get; set; } = new();

    public NetworkClient GetClient(int id)
    {
        Map.TryGetValue(id, out var value);
        return value;
    }
}