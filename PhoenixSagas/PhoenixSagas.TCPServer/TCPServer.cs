
namespace PhoenixSagas.TCPServer
{
    public class TcpNetworkServer
    {
        public TcpNetworkServer()
        {
                
        }

        public void Start()
        {
            Console.WriteLine("Running for 10 seconds then closing....");
            Thread.Sleep(7000);
            Console.WriteLine("Exiting.");
        }
    }
}
//using Confluent.Kafka;
//using ForgottenRealms.Kafka;
//using ForgottenRealms.Kafka.Implementations;
//using ForgottenRealms.Kafka.Interfaces;
//using ForgottenRealms.SharedCode.Implementations;
//using ForgottenRealms.SharedCode.Interfaces;
//using ForgottenRealms.SharedCode.Models;
//using ForgottenRealms.TcpNetworkServer.Interfaces;
//using ForgottenRealms.UtilityHelpers;
//using Kafka.Implementations;
//using Kafka.Interfaces;
//using SharedCode.SharedCode.Models;
//using System;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace ForgottenRealms.TCPSocketServer.Implementations
//{
//    public class TcpNetworkServer : ITcpNetworkServer
//    {

//        private Socket _socketListener;
//        private bool _serverRunning;
//        private Thread _connectionMonitor;
//        private readonly int _backlogSize = 250;
//        private readonly int _portNumber = 4000;
//        private readonly IConnectedClientsMap _clients;
//        private readonly IKafkaProducer<PlayerInputModel> _kafkaInputProducer;
//        private readonly IKafkaProducer<PlayerConnectionModel> _kafkaConnectionProducer;
//        private readonly IKafkaConsumer<PlayerOutputModel> _kafkaOutputConsumer;

//        public TcpNetworkServer(IConnectedClientsMap Clients)
//        {
//            _clients = Clients;
//            _kafkaInputProducer = new KafkaFactory().BuildProducer<PlayerInputModel>("PlayerInput");
//            _kafkaConnectionProducer = new KafkaProducer<PlayerConnectionModel>("PlayerConnect");
//            _kafkaOutputConsumer = new KafkaConsumer<PlayerOutputModel>("PlayerOutput");

//        }

//        public void Start()
//        {

//            SocketSetup();
//            _connectionMonitor = new Thread(ConnectionMonitor);
//            _connectionMonitor.Start();
//            _kafkaOutputConsumer.Start();
//            _kafkaOutputConsumer.MessagesHandler += PendingOutPutHandler;
//        }


//        private void SocketSetup()
//        {
//            Logger.Log($"Starting TCP server up on port: {_portNumber}");
//            try
//            {
//                _socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
//                _socketListener.Bind(new IPEndPoint(IPAddress.Any, _portNumber));
//                _socketListener.Listen(_backlogSize);
//            }
//            catch (Exception ex)
//            {
//                Logger.Error($"Error in SocketSetup - {ex.Message} - {ex.InnerException}");
//            }
//        }

//        public void ConnectionMonitor()
//        {
//            Logger.Log("Listening...");
//            _serverRunning = true;
//            while (_socketListener.IsBound && _serverRunning)
//            {
//                try
//                {
//                    ReadPendingInput();
//                    ManageDisconnects();
//                    AcceptNewConnections();
//                }
//                catch (Exception ex)
//                {
//                    Logger.Error("In Connection Monitor: " + ex.Message + " " + ex.InnerException);
//                }
//                if (Console.KeyAvailable)
//                {
//                    ConsoleKeyInfo key = Console.ReadKey();
//                    switch (key.KeyChar.ToString().ToUpperInvariant())
//                    {

//                        case "Q":
//                            ShutDown();
//                            break;
//                    }
//                }
//                Thread.Sleep(1);
//            }
//        }

//        private void ManageDisconnects()
//        {
//            lock (_clients.Map)
//            {
//                //foreach (var client in Clients()) // manage any disconnects      
//                for (int i = 0; i < _clients.Map.Count; i++)
//                {
//                    var item = _clients.Map.ElementAt(i);
//                    if (item.Value.client.GetType() == typeof(Socket))
//                    {
//                        var client = item.Value;
//                        if (client.PendingDisconnect)
//                            RemoveClient(client);
//                    }
//                }
//            }
//        }

//        private void AcceptNewConnections()
//        {
//            lock (_clients.Map)
//            {
//                try
//                {
//                    if (_socketListener.Poll(0, SelectMode.SelectRead))
//                        AddClient();
//                }
//                catch (Exception ex)
//                {
//                    if (!_socketListener.IsBound)
//                        Logger.Error($"Error polling socket, possibly on shutdown - {ex.Message} - {ex.InnerException}");

//                }
//            }
//        }

//        public async Task AddClient()
//        {
//            if (!_serverRunning)
//                return;

//            Socket client = _socketListener.Accept();
//            _socketListener.Blocking = false;
//            client.Blocking = true;
//            var tcpClient = new TcpNetworkClient<Socket>(client)
//            {
//                gameId = Guid.NewGuid()
//            };
//            lock (_clients.Map)
//            {
//                _clients.Map.TryAdd(client.Handle.ToInt32(), tcpClient);
//                Logger.Log($"Client Connected from: {tcpClient.IpAddress}");
//            }
//            var msg = new Message<Guid, PlayerConnectionModel>
//            {
//                Value = new PlayerConnectionModel(),
//                Key = tcpClient.gameId
//            };
//            msg.Value.gameId = tcpClient.gameId;
//            msg.Value.connected = true;
//            msg.Value.socketId = client.Handle.ToInt32();
//            await _kafkaConnectionProducer.Produce(msg);
//        }

//        public void RemoveClient(INetworkClient connectedClient)
//        {
//            lock (connectedClient)
//            {
//                try
//                {
//                    var client = (Socket)connectedClient.client;
//                    ProduceClientDisconnectMessage(_clients.GetClient(client.Handle.ToInt32()));
//                    _clients.Map.TryRemove(connectedClient.Handle, out var removedClient);
//                    client.Shutdown(SocketShutdown.Receive);
//                    client.Shutdown(SocketShutdown.Send);
//                    client.Disconnect(true);
//                    client.Dispose();
//                }
//                catch (Exception ex)
//                {
//                    Logger.Warn($"Failed to disconnect client during remove client method - {connectedClient.Handle} - \n\t\t{ex.Message} --- {ex.InnerException}");
//                    _clients.Map.TryRemove(connectedClient.Handle, out var removedClient);
//                    ProduceClientDisconnectMessage(connectedClient);
//                }
//            }
//        }

//        private void ReadPendingInput()
//        {
//            lock (_clients.Map)
//            {
//                for (int i = 0; i < _clients.Map.Count; i++)
//                {
//                    var item = _clients.Map.ElementAt(i);
//                    if (item.Value.client.GetType() == typeof(Socket))
//                    {
//                        var client = item.Value;
//                        if (!client.PendingDisconnect)
//                            ReceiveInput(client);
//                    }
//                }
//            }
//        }

//        public async Task ReceiveInput(INetworkClient connectedClient)
//        {
//            if (!_serverRunning)
//                return;

//            char[] line = new char[1024];

//            var client = (Socket)connectedClient.client;
//            if (!client.Connected && !connectedClient.PendingDisconnect)
//            {
//                connectedClient.PendingDisconnect = true;
//                return;
//            }
//            if (client.Poll(0, SelectMode.SelectRead))
//            {
//                try
//                {
//                    int bytesRead = connectedClient.Reader.Read(line, 0, 1024);
//                    if (bytesRead == 0)
//                        connectedClient.PendingDisconnect = true;
//                    else
//                        await SendInputToKafka(connectedClient.Handle, new NetworkClientData(new string(line)).GetOutputString());

//                }
//                catch (Exception ex)
//                {
//                    Logger.Error($"Error on receive, disconnecting connectedClient {connectedClient.Handle} \n\t\t{ex.Message} --- {ex.InnerException}");
//                    connectedClient.PendingDisconnect = true;
//                }
//            }
//        }

//        private async Task SendInputToKafka(int socketId, string message)
//        {
//            _clients.Map.TryGetValue(socketId, out INetworkClient connectedClient);
//            if (connectedClient == null)
//                return;

//            var msg = new Message<Guid, PlayerInputModel>
//            {
//                Key = connectedClient.gameId,
//                Value = new PlayerInputModel
//                {
//                    gameId = connectedClient.gameId,
//                    input = message,
//                    socketId = socketId
//                }
//            };
//            await _kafkaInputProducer.Produce(msg);
//        }

//        private void PendingOutPutHandler(object source, PlayerOutputModel e)
//        {
//            var socket = _clients.Map.Where(m => m.Value.gameId == e.gameId).Select(x => x.Key).FirstOrDefault();
//            _clients.Map.TryGetValue(socket, out var client);
//            SendQueuedOutput(client, e.output);
//        }


//        public async Task SendQueuedOutput(INetworkClient client, string msg)
//        {
//            if (!_serverRunning || client == null || client.PendingDisconnect)
//                return;

//            try
//            {
//                StringBuilder sendPacket = new StringBuilder();
//                lock (client)
//                {
//                    sendPacket.Append(msg);
//                    sendPacket.Append("\n\r");
//                }
//                await SendDirectOutput(sendPacket.ToString(), client);
//            }
//            catch (Exception ex)
//            {
//                Logger.Error($"Error in TCPNetworkServer - Sendoutput {client.Handle} - {client.IpAddress}\n\t\t{ex.Message} --- {ex.InnerException}");
//                RemoveClient(client);
//            }
//        }
//        public async Task SendDirectOutput(string message, INetworkClient client)
//        {
//            if (client == null)
//                return;

//            var c = (Socket)client.client;
//            if (!c.Connected)
//            {
//                client.PendingDisconnect = true;
//                return;
//            }
//            try
//            {
//                await client.Writer.WriteLineAsync(message);
//                client.Writer.Flush();
//            }
//            catch (Exception ex)
//            {
//                Logger.Error($"Error in SocktServer - SendDirectOutput, disconnecting connectedClient: {client.Handle} - {client.IpAddress}\n\t\t{ex.Message} --- {ex.InnerException}");
//                client.PendingDisconnect = true;
//                RemoveClient(client);
//            }

//        }

//        public void ProduceClientDisconnectMessage(INetworkClient client)
//        {
//            Logger.Log($"A connectedClient has disconnected {client.Handle} - {client.IpAddress}");
//            if (!_serverRunning)
//                return;

//            client.PendingDisconnect = true;
//            _clients.Map.TryGetValue(client.Handle, out INetworkClient connectedClient);

//            if (connectedClient == null)
//                return;

//            var msg = new Message<Guid, PlayerConnectionModel>
//            {
//                Key = connectedClient.gameId,
//                Value = new PlayerConnectionModel()
//                {
//                    gameId = connectedClient.gameId,
//                    socketId = client.Handle,
//                    connected = false
//                }
//            };
//            _kafkaConnectionProducer.Produce(msg);
//        }

//        public void ShutDown()
//        {
//            _kafkaOutputConsumer.Shutdown();
//            Thread.Sleep(3000);
//            _serverRunning = false;

//            lock (_clients.Map)
//            {
//                foreach (var connectedClient in _clients.Map.Values.Where(x => x.client.GetType() == typeof(Socket)).Select(x => x.client))
//                {
//                    var client = (Socket)connectedClient;
//                    if (client.Connected)
//                    {
//                        lock (client)
//                        {
//                            try
//                            {
//                                Console.WriteLine($"Removing client: {client.Handle}");
//                                RemoveClient(_clients.GetClient(client.Handle.ToInt32()));
//                            }
//                            catch (Exception ex)
//                            {
//                                Logger.Error("Error disconnecting clients on shutdown: " + ex.Message + " " + ex.InnerException);
//                            }
//                        }
//                    }
//                }
//            }
//            Logger.Log("Shutting down socket server...");
//            try
//            {
//                _socketListener.Close();
//                _socketListener.Dispose();
//                Thread.Sleep(2000);
//                _clients.Map.Clear();
//            }
//            catch (Exception ex)
//            {
//                Logger.Error($"Error closing and disposing of socket, {ex.Message} - {ex.InnerException}, exiting anyway.");
//            }

//            Logger.Log("Socket server shutdown complete.");
//        }
//    }
//}
