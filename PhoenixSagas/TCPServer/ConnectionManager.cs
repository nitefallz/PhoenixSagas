using PhoenixSagas.Kafka.Implementations;
using PhoenixSagas.Kafka.Interfaces;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using PhoenixSagas.Models;

namespace PhoenixSagas.TCPSocketServer.Implementations
{
    public class ConnectionManager
    {
        private readonly ConcurrentDictionary<int, TcpClient> _clients = new();
    
        private readonly IKafkaProducer<PlayerInput> _kafkaInputProducer;
        private readonly IKafkaProducer<PlayerConnection> _kafkaConnectionProducer;
        private readonly IKafkaConsumer<PlayerOutput> _kafkaOutputConsumer;

        public ConnectionManager(IKafkaProducer<PlayerInput> inputProducer, KafkaConsumer<PlayerOutput> outputConsumer)
        {
            _kafkaInputProducer = inputProducer;
            _kafkaOutputConsumer = outputConsumer;
        }

        public void HandleNewConnection(Socket socket)
        {
            var client = new TcpClient { Client = socket };
            _clients.TryAdd(socket.Handle.ToInt32(), client);
            Task.Run(() => ReadClientInput(client));
        }

        private async Task ReadClientInput(TcpClient client)
        {
            var stream = client.GetStream();
            var buffer = new byte[1024];
            while (true) // Add a condition to exit the loop based on your logic
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                //await _kafkaInputProducer.Produce(message);
            }
            // Handle client disconnection
        }

        // Method to send messages to clients based on Kafka consumer messages
        // This should be invoked by the KafkaConsumerService message handler
    }
}