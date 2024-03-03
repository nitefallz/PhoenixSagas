using Confluent.Kafka;
using System.Threading.Tasks;

namespace PhoenixSagas.TCPSocketServer.Implementations
{
    public class KafkaProducerService<T>
    {
        private readonly string _topic;
        private readonly IProducer<Null, string> _producer;

        public KafkaProducerService(string bootstrapServers, string topic)
        {
            _topic = topic;
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task SendMessageAsync(string message)
        {
            Console.WriteLine("Sending to Kafka");
            //await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = message });
        }
    }
}