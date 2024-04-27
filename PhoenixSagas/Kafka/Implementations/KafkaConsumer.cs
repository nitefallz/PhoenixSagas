using Confluent.Kafka;
using PhoenixSagas.Kafka.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PhoenixSagas.Kafka.Implementations
{
    public class KafkaConsumer<T> : IKafkaConsumer<T> where T : new()
    {
        private readonly string _topic;
        private readonly IConsumer<Guid, T> _messageConsumer;
        public EventHandler<T> _messagesHandler;
        private readonly CancellationTokenSource _cancelToken = new CancellationTokenSource();

        public KafkaConsumer(string topic, EventHandler<T> messageHandler)
        {
            try
            {
                _topic = topic;
                _messagesHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));

                var conf = new ConsumerConfig
                {
                    GroupId = "phoenixsagas",
                    BootstrapServers = "gomezdev.hopto.org:29092",
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = true
                };
                _messageConsumer = new ConsumerBuilder<Guid, T>(conf)
                    .SetKeyDeserializer(new GuidDeserializer())
                    .SetValueDeserializer(new OutputDeserializer<T>())
                    .Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
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
                    _messagesHandler?.Invoke(this, consumeResult.Message.Value);
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

        public void Shutdown()
        {
            _cancelToken.Cancel();
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
