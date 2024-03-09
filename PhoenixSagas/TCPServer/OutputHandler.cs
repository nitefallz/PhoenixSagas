using PhoenixSagas.Kafka.Implementations;
using PhoenixSagas.Kafka.Interfaces;
using PhoenixSagas.Kafka.Implementations;
using PhoenixSagas.Models;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PhoenixSagas.TCPServer.Implementations
{
    public class OutputHandler : IMessageHandler<PlayerOutput>
    {
        private readonly IConnectedClientsMap _clients;

        public OutputHandler(IConnectedClientsMap clients)
        {
            _clients = clients ?? throw new ArgumentNullException(nameof(clients));
        }

        public async Task HandleMessageAsync(PlayerOutput message, CancellationToken cancellationToken)
        {
            // Assuming PlayerOutput contains enough information to identify the target client
            if (_clients.Map.TryGetValue(message.socketId, out NetworkClient client))
            {
                var messageBytes = Encoding.UTF8.GetBytes(message.output); // Convert message to bytes
                await client.client.GetStream().WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
            }
        }
    }

}