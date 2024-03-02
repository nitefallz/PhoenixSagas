using Confluent.Kafka;
using PhoenixSagas.Kafka.Interfaces;
using System;
using System.Threading.Tasks;

namespace PhoenixSagas.Kafka.Implementations
{

    public class KafkaProducer<T> : IKafkaProducer<T> where T : new()
    {
        private readonly ProducerConfig _config = new ProducerConfig { BootstrapServers = "gomezdev.hopto.org:29092", LingerMs = 0, Acks = Acks.All,CompressionType = CompressionType.None,BatchSize = 1,EnableIdempotence = true};
        private readonly string _topic;
        private readonly IProducer<Guid, T> _producer;
        public EventHandler<T> MessagesHandler { get; set; }

        public KafkaProducer(string Topic)
        {
            _topic = Topic;
            _producer = new ProducerBuilder<Guid, T>(_config).SetKeySerializer(new GuidSerializer()).SetValueSerializer(new JsonSerializer<T>()).Build();
        }

        public async Task Produce(Message<Guid, T> message)
        {
            try
            {
                await _producer.ProduceAsync(_topic, message);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Oops, something went wrong: {e}");
            }
        }
    }
}