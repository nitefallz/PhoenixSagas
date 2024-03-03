using System;

namespace PhoenixSagas.Models
{
    [Serializable]
    public class PlayerOutput
    {
        public int socketId { get; set; }
        public Guid gameId { get; set; }
        public string output { get; set; }

        public PlayerOutput()
        {
            gameId = new Guid();
        }
    }
}