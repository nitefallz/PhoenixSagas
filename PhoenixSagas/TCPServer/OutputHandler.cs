using PhoenixSagas.Kafka.Interfaces;
using PhoenixSagas.Models;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PhoenixSagas.TcpServer.Implementations
{
    public class OutputHandler : IMessageHandler<PlayerOutput>
    {
        private readonly IConnectedClientsMap _clients;
        private readonly ILogger<OutputHandler> _logger;

        public OutputHandler(IConnectedClientsMap clients, ILogger<OutputHandler> logger)
        {
            _clients = clients ?? throw new ArgumentNullException(nameof(clients));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
    }
}