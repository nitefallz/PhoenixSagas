using PhoenixSagas.TcpServer.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

}
