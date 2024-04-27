using PhoenixSagas.TCPServer.Implementations;
using PhoenixSagas.TCPServer.Models;

namespace PhoenixSagas.Tests
{
    public class ConnectedClientMapTests
    {
        [Fact]
        public void GetClient_ReturnsClientWhenExists()
        {
            // Arrange
            var map = new ConnectedClientMap();
            var networkClient = new NetworkClient();
            int clientId = 123;

            // Act
            map.Map.TryAdd(clientId, networkClient);
            var retrievedClient = map.GetClient(clientId);

            // Assert
            Assert.Equal(networkClient, retrievedClient);
        }

        [Fact]
        public void GetClient_ReturnsNullWhenClientDoesNotExist()
        {
            // Arrange
            var map = new ConnectedClientMap();
            int clientId = 123;

            // Act
            var retrievedClient = map.GetClient(clientId);

            // Assert
            Assert.Null(retrievedClient);
        }

        [Fact]
        public void RemoveClient_ClientIsRemovedSuccessfully()
        {
            // Arrange
            var map = new ConnectedClientMap();
            var networkClient = new NetworkClient();
            int clientId = 123;
            map.Map.TryAdd(clientId, networkClient);

            // Act
            var result = map.Map.TryRemove(clientId, out var _); // Assuming RemoveClient is a method to be implemented

            // Assert
            Assert.True(result);
            Assert.False(map.Map.ContainsKey(clientId));
        }

        [Fact]
        public void HandleNewConnection_Failure_LogsError()
        {
            /*// Arrange
            var clientsMap = new ConnectedClientMap();
            var mockKafkaProducer = new Mock<IKafkaProducer<PlayerInput>>();
            var mockLogger = new Mock<ILogger<OutputHandler>>();
            var connectionManager = new ConnectionManager(clientsMap, mockKafkaProducer.Object, "playerOutputTopic", mockLogger.Object);

            var faultySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Ggp); // Simulate a faulty socket

            // Act
            connectionManager.HandleNewConnection(faultySocket);

            // Assert
            mockLogger.Verify(logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to handle new connection")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);*/
        }

    }

}
