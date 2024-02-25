using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixSagas.TCPSocketServer.Implementations
{
    public class TcpNetworkServer
    {
        private Socket _listenerSocket;
        private readonly ConcurrentDictionary<int, TcpClient> _connectedClients = new ConcurrentDictionary<int, TcpClient>();
        private readonly int _portNumber = 4000;
        private bool _isRunning;

        public TcpNetworkServer()
        {
            _listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            _listenerSocket.Bind(new IPEndPoint(IPAddress.Any, _portNumber));
            _listenerSocket.Listen(100);
            _isRunning = true;
            Console.WriteLine($"Server started on port {_portNumber}.");

            Task.Run(() => AcceptConnections());

            Console.WriteLine("Press 'q' to quit.");
            while (Console.Read() != 'q')
            {
                // You can implement additional console commands here
            }

            ShutDown();
        }


        private void AcceptConnections()
        {
            while (_isRunning)
            {
                try
                {
                    // Accept a connection
                    Socket clientSocket = _listenerSocket.Accept();
                    Task.Run(() => ProcessClient(clientSocket));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Accept connection error: {ex.Message}");
                }
            }
        }

        private async Task ProcessClient(Socket clientSocket)
        {
            var clientId = clientSocket.Handle.ToInt32();
            Console.WriteLine($"Client connected: {clientId}");

            var buffer = new byte[1024];
            var clientStream = new NetworkStream(clientSocket);

            try
            {
                while (_isRunning && clientSocket.Connected)
                {
                    var bytesRead = await clientStream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        var receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received from {clientId}: {receivedText}");

                        // Echo back the received message to the client
                        await clientStream.WriteAsync(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error with client {clientId}: {ex.Message}");
            }
            finally
            {
                clientStream.Close();
                clientSocket.Close();
                Console.WriteLine($"Client disconnected: {clientId}");
            }
        }

        public void ShutDown()
        {
            Console.WriteLine("Shutting down server...");
            _isRunning = false;
            _listenerSocket.Close();
            // Close all client connections
            foreach (var client in _connectedClients.Values)
            {
                client.Close();
            }
            _connectedClients.Clear();
            Console.WriteLine("Server shutdown complete.");
        }
    }
}
