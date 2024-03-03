using PhoenixSagas.TCPSocketServer.Implementations;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixSagas.TCPSocketServer.Implementations
{
    public class TcpNetworkServer
    {
        private readonly Socket _listenerSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly ConnectionManager _connectionManager;
        private readonly int _port = 4000;
        private bool _isRunning;

        public TcpNetworkServer(ConnectionManager connectionManager)
        {
            _connectionManager = connectionManager;
        }

        public void Start()
        {
            _listenerSocket.Bind(new IPEndPoint(IPAddress.Any, _port));
            _listenerSocket.Listen(120); // Adjust the backlog as necessary
            _isRunning = true;
            Console.WriteLine($"Server started on port {_port}.");
            AcceptConnectionsAsync();
            MonitorShutdownCommand();
        }

        private async void AcceptConnectionsAsync()
        {
            while (_isRunning)
            {
                try
                {
                    var clientSocket = await _listenerSocket.AcceptAsync();
                    _connectionManager.HandleNewConnection(clientSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Accept connection error: {ex.Message}");
                    if (!_isRunning) break;
                }
            }
        }

        public async Task SendClientOutput(int clientId, string output)
        {
            //if (_connectedClients.TryGetValue(clientId, out var clientStream))
            {
                var buffer = Encoding.UTF8.GetBytes(output);
              //  await clientStream.WriteAsync(buffer, 0, buffer.Length);
                // Consider adding error handling and logging here
            }
            //else
            {
                Console.WriteLine($"Client {clientId} not found.");
                // Handle the case where the client is not connected anymore
            }
        }


        private void MonitorShutdownCommand()
        {
            Task.Run(() =>
            {
                Console.WriteLine("Press 'q' to shut down the server.");
                while (_isRunning)
                {
                    if (Console.ReadKey().KeyChar == 'q')
                    {
                        Console.WriteLine("\nShutting down...");
                        ShutDown();
                        break;
                    }
                }
            });
        }

        public void ShutDown()
        {
            _isRunning = false;
            _listenerSocket.Close();
            Console.WriteLine("Server stopped.");
        }
    }
}
