using System;
using ForgottenRealms.SharedCode.Models;

namespace Kafka.Interfaces
{
    public interface IKafkaInputConsumer
    {
        public void Start();
        void ReadMessagesAsync();
        public void Shutdown();
        EventHandler<PlayerInputModel> InputReceived { get; set; }
    }
}
