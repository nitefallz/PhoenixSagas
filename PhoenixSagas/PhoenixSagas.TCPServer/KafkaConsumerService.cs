using Confluent.Kafka;
using System;
using System.Threading;

namespace PhoenixSagas.TCPSocketServer.Implementations
{
    public class KafkaConsumerService<T>
    {
        private readonly string _topic;
        private readonly IConsumer<Null, string> _consumer;
        private readonly Action<string> _messageHandler;

        public KafkaConsumerService(string bootstrapServers, string topic, string groupId, Action<string> messageHandler)
        {
            _topic = topic;
            _messageHandler = messageHandler;
            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrapServers,
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _consumer = new ConsumerBuilder<Null, string>(config).Build();
        }

        public void Start(CancellationToken cancellationToken)
        {
            _consumer.Subscribe(_topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(cancellationToken);
                _messageHandler(consumeResult.Message.Value);
            }
        }
    }
}