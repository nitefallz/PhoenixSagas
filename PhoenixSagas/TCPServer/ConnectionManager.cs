using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PhoenixSagas.TCPSocketServer.Implementations  
{
    public class ConnectionManager
    {
        private readonly ConcurrentDictionary<int, TcpClient> _clients = new();
        private readonly KafkaProducerService<string> _inputProducer;
        private readonly KafkaConsumerService<string> _outputConsumer;

        public ConnectionManager(KafkaProducerService<string> inputProducer, KafkaConsumerService<string> outputConsumer)
        {
            _inputProducer = inputProducer;
            _outputConsumer = outputConsumer;
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
                await _inputProducer.SendMessageAsync(message);
            }
            // Handle client disconnection
        }

        // Method to send messages to clients based on Kafka consumer messages
        // This should be invoked by the KafkaConsumerService message handler
    }
}