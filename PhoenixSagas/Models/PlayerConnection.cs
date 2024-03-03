using System;

namespace PhoenixSagas.Models
{
    [Serializable]
    public class PlayerConnection
    {
        public int socketId { get; set; }
        public Guid gameId { get; set; }
        public bool connected { get; set; }

        public PlayerConnection()
        {
            gameId = new Guid();
        }
    }
}