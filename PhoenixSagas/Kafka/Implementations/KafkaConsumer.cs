using Confluent.Kafka;
using System;
using System.Threading;
using System.Threading.Tasks;
using PhoenixSagas.Kafka.Interfaces;

namespace PhoenixSagas.Kafka.Implementations
{
    public class KafkaConsumer<T> : IKafkaConsumer<T> where T : new() //IDisposable where T : new()
    {
        private readonly string _topic;
        private IConsumer<Guid, T> _messageConsumer;
        private readonly ConsumerConfig _conf;
        private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();
        private readonly IMessageHandler<T> _messageHandler;

        public KafkaConsumer(string topic, IMessageHandler<T> messageHandler)
        {
            _topic = topic;
            _messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
            _conf = new ConsumerConfig
            {
                GroupId = "phoenixsagas",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };
            _messageConsumer = new ConsumerBuilder<Guid, T>(_conf)
                                .SetKeyDeserializer(new GuidDeserializer())
                                .SetValueDeserializer(new OutputDeserializer<T>())
                                .Build();
        }

        public void Start()
        {
            Task.Run(() => ConsumeMessagesAsync(_cancelToken.Token));
        }

        private async Task ConsumeMessagesAsync(CancellationToken cancellationToken)
        {
            _messageConsumer.Subscribe(_topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _messageConsumer.Consume(cancellationToken);
                    await _messageHandler.HandleMessageAsync(consumeResult.Message.Value, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break; // Graceful exit
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error consuming message: {ex.Message}");
                }
            }

            _messageConsumer.Close(); // Ensure the consumer is properly closed when finished
        }

        public void Dispose()
        {
            _cancelToken.Cancel();
            _messageConsumer?.Dispose();
            _cancelToken?.Dispose();
        }
    }

    // Include implementations for GuidDeserializer and CustomDeserializer<T> as needed.
}
