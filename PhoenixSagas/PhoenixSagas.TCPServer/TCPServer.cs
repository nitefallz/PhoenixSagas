
//namespace PhoenixSagas.TCPServer
//{
//    public class TcpNetworkServer
//    {
//        public TcpNetworkServer()
//        {

//        }

//        public void Start()
//        {
//            Console.WriteLine("Running for 5 seconds then closing....");
//            Thread.Sleep(5000);
//            Console.WriteLine("Exiting.");
//        }
//    }

using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhoenixSagas.TCPSocketServer.Implementations
{
    public class TcpNetworkServer
    {
        private Socket _socketListener;
        private bool _serverRunning;
        private Thread _connectionMonitor;
        private readonly int _backlogSize = 250;
        private readonly int _portNumber = 4000;
        private readonly Dictionary<int, TcpClient> _connectedClients = new Dictionary<int, TcpClient>();

        public TcpNetworkServer()
        {
        }

        public void Start()
        {
            SocketSetup();
            _connectionMonitor = new Thread(ConnectionMonitor);
            _connectionMonitor.Start();
        }

        private void SocketSetup()
        {
            Console.WriteLine($"Starting TCP server up on port: {_portNumber}");
            _socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socketListener.Bind(new IPEndPoint(IPAddress.Any, _portNumber));
            _socketListener.Listen(_backlogSize);
        }

        private void ConnectionMonitor()
        {
            Console.WriteLine("Listening...");
            _serverRunning = true;
            while (_serverRunning)
            {
                try
                {
                    if (_socketListener.Poll(1000, SelectMode.SelectRead))
                    {
                        var clientSocket = _socketListener.Accept();
                        var clientId = clientSocket.Handle.ToInt32();
                        _connectedClients.Add(clientId, new TcpClient(clientSocket));
                        Console.WriteLine($"Client connected: {clientId}");

                        Task.Run(() => HandleClient(clientId));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection Monitor Error: {ex.Message}");
                }
            }
        }

        private async Task HandleClient(int clientId)
        {
            if (!_connectedClients.TryGetValue(clientId, out var client))
            {
                return;
            }

            var buffer = new byte[1024];
            try
            {
                while (_serverRunning && client.Connected)
                {
                    var bytesRead = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        var receivedText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Received from {clientId}: {receivedText}");

                        // Echo back the received message to the client
                        await client.GetStream().WriteAsync(buffer, 0, bytesRead);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Handle Client Error: {ex.Message}");
            }
            finally
            {
                client.Close();
                _connectedClients.Remove(clientId);
                Console.WriteLine($"Client disconnected: {clientId}");
            }
        }

        public void ShutDown()
        {
            Console.WriteLine("Shutting down socket server...");
            _serverRunning = false;
            _socketListener.Close();
            foreach (var client in _connectedClients.Values)
            {
                client.Close();
            }
            _connectedClients.Clear();
            Console.WriteLine("Socket server shutdown complete.");
        }
    }

    public class TcpClient : IDisposable
    {
        private Socket _clientSocket;

        public bool Connected => _clientSocket.Connected;

        public TcpClient(Socket clientSocket)
        {
            _clientSocket = clientSocket;
        }

        public NetworkStream GetStream()
        {
            return new NetworkStream(_clientSocket);
        }

        public void Close()
        {
            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}

