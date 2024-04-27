using PhoenixSagas.Interfaces.GameEngine;

namespace PhoenixSagas.Implementations.GameEngine
{
    public class GameEngine : IGameEngine
    {
        public Task Start(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void ShutDown()
        {
            throw new NotImplementedException();
        }
    }
}