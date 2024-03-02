namespace PhoenixSagas.Kafka.Interfaces
{
    public interface IKafkaFactory
    {
        IKafkaProducer<T> BuildProducer<T>(string topic) where T : new();
        IKafkaConsumer<T> BuildConsumer<T>(string topic) where T : new();
    }
}
