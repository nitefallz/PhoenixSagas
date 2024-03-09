﻿using Confluent.Kafka;
using PhoenixSagas.Kafka.Implementations;
using PhoenixSagas.Kafka.Interfaces;
using PhoenixSagas.Models;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixSagas.TCPServer.Implementations
{
    public class NetworkClient
    {
        public TcpClient client { get; set; }
        public bool PendingDisconnect { get; set; }
        public int Handle { get; set; }
        public bool InGame { get; set; }
        public Guid gameId { get; set; }
    }

    public interface IConnectedClientsMap
    {
        ConcurrentDictionary<int, NetworkClient> Map { get; set; }
        NetworkClient GetClient(int id);
    }

    public class ConnectedClientMap : IConnectedClientsMap
    {
        public ConcurrentDictionary<int, NetworkClient> Map { get; set; } = new();

        public NetworkClient GetClient(int id)
        {
            Map.TryGetValue(id, out var value);
            return value;
        }
    }
    public class ConnectionManager
    {
        private readonly IConnectedClientsMap _clients;
        private readonly IKafkaProducer<PlayerInput> _kafkaInputProducer;
        private readonly IKafkaConsumer<PlayerOutput> _kafkaOutputConsumer;

        public ConnectionManager(IConnectedClientsMap clients)
        {
            _kafkaInputProducer = new KafkaFactory().BuildProducer<PlayerInput>("PlayerInput");
            _clients = clients ?? throw new ArgumentNullException(nameof(clients));
            _kafkaOutputConsumer = new KafkaConsumer<PlayerOutput>("PlayerOutput");
        }

        public void HandleNewConnection(Socket socket)
        {
            var tcpclient = new NetworkClient { client = new TcpClient { Client = socket }, Handle = socket.Handle.ToInt32(), InGame = false, gameId = Guid.NewGuid(), PendingDisconnect = false };
            var clientId = socket.Handle.ToInt32();
            _clients.Map.TryAdd(clientId, tcpclient);
            Task.Run(() => ReadClientInput(tcpclient, clientId));
        }

        private async Task ReadClientInput(NetworkClient client, int clientId)
        {
            var stream = client.client.GetStream();
            var buffer = new byte[1024];
            while (client.client.Connected)
            {
                try
                {
                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0)
                    {
                        Console.WriteLine($"Client {clientId} disconnected.");
                        break;
                    }
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    var msg = new Message<Guid, PlayerInput>
                    {
                        Key = client.gameId,
                        Value = new PlayerInput
                        {
                            gameId = client.gameId,
                            input = message,
                            socketId = client.Handle
                        }
                    };
                    await _kafkaInputProducer.Produce(msg);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading from client {clientId}: {ex.Message}");
                    break;
                }
            }

            if (_clients.Map.TryRemove(clientId, out var _))
            {
                client.client.Close();
                client.client.Dispose();
            }
        }
    }
}
