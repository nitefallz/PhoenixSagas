using Microsoft.Extensions.Logging;
using PhoenixSagas.Kafka.Interfaces;
using PhoenixSagas.Models;
using PhoenixSagas.TCPServer.Interfaes;
using PhoenixSagas.TCPServer.Models;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhoenixSagas.TCPServer.Implementations
{
    public class OutputHandler :IOutputHandler
    {
        private readonly IConnectedClientsMap _clients;
        private readonly ILogger<OutputHandler> _logger;
        public readonly IKafkaConsumer<PlayerOutput> _kafkaOutputConsumer;

        public OutputHandler(IConnectedClientsMap clients, ILogger<OutputHandler> logger, IKafkaFactory kafkaFactory)
        {
            _clients = clients ?? throw new ArgumentNullException(nameof(clients));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            kafkaFactory.BuildConsumer<PlayerOutput>("PlayerOutput", delegate(object o, PlayerOutput output) { OnOutputReceived(o, output); });
        }

        public void Start(CancellationToken cancellationToken)
        {
            _kafkaOutputConsumer.Start();
        }
        public void ShutDown()
        {
            _kafkaOutputConsumer.Shutdown();
        }

        public async Task HandleMessageAsync(PlayerOutput message, CancellationToken cancellationToken)
        {
            if (_clients.Map.TryGetValue(message.socketId, out NetworkClient client))
            {
                try
                {
                    var messageBytes = Encoding.UTF8.GetBytes(message.output); // Convert message to bytes
                    await client.client.GetStream().WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
                    _logger.LogInformation($"Sent message to client {message.socketId}.");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error sending message to client {message.socketId}: {ex.Message}");
                    // Consider handling the client disconnection or retry logic here
                }
            }
            else
            {
                _logger.LogWarning($"Client {message.socketId} not found.");
            }
        }

        private async Task OnOutputReceived(object? source, PlayerOutput e)
        {
            await HandleMessageAsync(e, CancellationToken.None);
        }
    }
}