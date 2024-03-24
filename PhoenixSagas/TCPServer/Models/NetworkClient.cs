using System;
using System.Net.Sockets;

namespace PhoenixSagas.TCPServer.Models;

public class NetworkClient
{
    public TcpClient client { get; set; }
    public bool PendingDisconnect { get; set; }
    public int Handle { get; set; }
    public bool InGame { get; set; }
    public Guid gameId { get; set; }
}