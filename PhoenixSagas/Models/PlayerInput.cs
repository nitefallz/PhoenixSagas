using System;

namespace PhoenixSagas.Models
{
    [Serializable]
    public class PlayerInput
    {
        public int socketId { get; set; }
        public Guid gameId { get; set; }
        public string input { get; set; }

        public PlayerInput()
        {
            gameId = new Guid();
        }
    }
}