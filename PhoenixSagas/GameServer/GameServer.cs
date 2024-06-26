﻿using PhoenixSagas.Kafka.Interfaces;
using PhoenixSagas.Models;

// Mananges game engine and state

namespace PhoenixSagas.GameServer
{
    public class GameServer : IGameServer
    {
        private readonly IKafkaConsumer<PlayerInput> _playerInputConsumer;
        private readonly IGameEngine _gameEngine;

        public GameServer(IKafkaFactory kafkaFactory, IGameEngine gameEngine)
        {
            _playerInputConsumer = kafkaFactory.BuildConsumer<PlayerInput>("PlayerInput", OnInputReceived);
            _gameEngine = gameEngine;
        }

        public void Start(CancellationToken cancellationToken)
        {
            _playerInputConsumer.Start();
        }


        private void OnInputReceived(object? source, PlayerInput e)
        {
            if (e == null)
                return;

            Console.WriteLine($"Input: {e.input}");
        }
        
        public void ShutDown()
        {
            _playerInputConsumer.Shutdown();
        }
    }
}
