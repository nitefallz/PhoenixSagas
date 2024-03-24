using PhoenixSagas.TCPServer.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace PhoenixSagas.TCPServer.Implementations
{
    public class TcpNetworkServer : ITcpNetworkServer
    {
        private readonly Socket _listenerSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly IConnectionManager _connectionManager;
        private readonly int _port = 4000;
        private CancellationTokenSource _cts = new();

        public TcpNetworkServer(IConnectionManager connectionManager, int port = 4000)
        {
            _connectionManager = connectionManager ?? throw new ArgumentNullException(nameof(connectionManager));
            _port = port;
        }

        public async Task Start(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            _listenerSocket.Bind(new IPEndPoint(IPAddress.Any, _port));
            _listenerSocket.Listen(120); 
            Console.WriteLine($"Server started on port {_port}.");

            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    var clientSocket = await _listenerSocket.AcceptAsync(cancellationToken);
                    _connectionManager.HandleNewConnection(clientSocket);
                }
                catch (Exception ex) when (ex is SocketException or ObjectDisposedException)
                {
                    Console.WriteLine($"Accept connection error: {ex.Message}");
                    if (_listenerSocket is { IsBound: false }) break;
                }
            }
        }

        public void ShutDown()
        {
            _cts.Cancel();
            _listenerSocket.Close();
            Console.WriteLine("Server stopped.");
        }
    }
}
