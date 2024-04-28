using PhoenixSagas.Kafka.Interfaces;
using System;

namespace PhoenixSagas.Kafka.Implementations
{
    public class KafkaFactory : IKafkaFactory
    {
        public IKafkaProducer<T> BuildProducer<T>(string topic) where T : new() => new KafkaProducer<T>(topic);
        public IKafkaConsumer<T> BuildConsumer<T>(string topic, EventHandler<T> messageHandler) where T : new() => new KafkaConsumer<T>(topic, messageHandler);
    }
}
