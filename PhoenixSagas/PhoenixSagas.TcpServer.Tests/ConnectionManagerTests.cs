using Microsoft.Extensions.Logging;
using Moq;
using PhoenixSagas.Kafka.Interfaces;
using PhoenixSagas.Models;
using PhoenixSagas.TCPServer.Implementations;
using PhoenixSagas.TCPServer.Interfaes;
using System.Net.Sockets;

namespace PhoenixSagas.Tests
{
    public class ConnectionManagerTests
    {
        [Fact]
        public void HandleNewConnection_AddsClientToMap()
        {
            // Arrange
            var clientsMap = new ConnectedClientMap(); // Use the real implementation for integration testing
            var mockKafkaProducer = new Mock<IKafkaProducer<PlayerInput>>();
            var mockLogger = new Mock<ILogger<OutputHandler>>();

            // Although we're testing the concrete ConnectionManager, it's designed to implement IConnectionManager
            var connectionManager = new ConnectionManager(clientsMap, mockLogger.Object);

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // Ensure your method of generating client IDs aligns with ConnectionManager's approach
            var clientId = socket.Handle.ToInt32();

            // Act
            // Directly invoke the method on the concrete class through its interface
            connectionManager.HandleNewConnection(socket);

            // Assert
            // Verify the client was indeed added as expected
            Assert.True(clientsMap.Map.ContainsKey(clientId));
        }
    }

}
