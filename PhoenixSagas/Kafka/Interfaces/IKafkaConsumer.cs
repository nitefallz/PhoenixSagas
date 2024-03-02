using System;

namespace PhoenixSagas.Kafka.Interfaces
{
    public interface IKafkaConsumer<T> : IDisposable where T : new()
    {
        public void Start();
        void ReadMessagesAsync();
        public void Shutdown();
        EventHandler<T> MessagesHandler { get; set; }
    }
}
