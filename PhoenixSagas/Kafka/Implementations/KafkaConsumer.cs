using Confluent.Kafka;
using PhoenixSagas.Kafka.Interfaces;
using System;
using System.Threading;

namespace PhoenixSagas.Kafka.Implementations
{

    public class KafkaConsumer<T> : IKafkaConsumer<T> where T : new()
    {
        public EventHandler<T> MessagesHandler { get; set; }
        private readonly string _topic;
        private readonly Thread _threadMonitor;
        private IConsumer<Guid, T> _messageConsumer;
        readonly CancellationTokenSource _cancelToken;
        private readonly ConsumerConfig _conf;
        private bool _running;

        public KafkaConsumer(string Topic)
        {

            _topic = Topic;
            _running = false;
            _threadMonitor = new Thread(ReadMessagesAsync);
            _cancelToken = new CancellationTokenSource();
            _conf = new ConsumerConfig
            {
                GroupId = "phoenixsagas",
                BootstrapServers = "gomezdev.hopto.org:29092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                EnableAutoOffsetStore = false,
                //xPollIntervalMs = 10000,
                //SessionTimeoutMs = 300000,
                AllowAutoCreateTopics = true
            };

            using (_messageConsumer = new ConsumerBuilder<Guid, T>(_conf)
                       .SetKeyDeserializer(new GuidDeserializer())
                       .SetValueDeserializer(new OutputDeserializer<T>())
                       .SetPartitionsAssignedHandler((c, partitions) =>
                       {
                           // Handle partitions assigned, e.g., seek to specific offsets
                       })
                       .SetPartitionsRevokedHandler((c, partitions) =>
                       {
                           // Commit offsets before rebalance
                           try
                           {
                               c.Commit();
                           }
                           catch (KafkaException e)
                           {
                               Console.WriteLine($"Commit error before rebalance: {e.Error.Reason}");
                           }
                       })
                       .Build()) ;
        }

        public void Start() => _threadMonitor.Start();

        public void Shutdown()
        {
            _running = false;
            Thread.Sleep(500);
            _cancelToken.Cancel();
        }

        public void ReadMessagesAsync()
        {
            _running = true;
            try
            {
                using (_messageConsumer = new ConsumerBuilder<Guid, T>(_conf).SetKeyDeserializer(new GuidDeserializer()).SetValueDeserializer(new OutputDeserializer<T>()).Build())
                {
                    TopicPartition tp = new TopicPartition(_topic, 0);
                    _messageConsumer.Assign(tp);
                    _messageConsumer.Subscribe(_topic);

                    while (_running)
                    {
                        try
                        {
                            var messagesRead = _messageConsumer.Consume(TimeSpan.FromMilliseconds(_conf.MaxPollIntervalMs - 1000 ?? 15000));
                            if (messagesRead == null)
                                continue;
                            //Console.WriteLine($"new Kafka message for {_topic}, offset - {messagesRead.Offset}");
                            MessagesHandler?.Invoke(this, messagesRead.Message.Value);

                            try
                            {
                                //Console.WriteLine("Comitting offsets...");
                                _messageConsumer.Commit(messagesRead);
                                _messageConsumer.StoreOffset(messagesRead);
                                //Console.WriteLine($"Commited offset, and stored {messagesRead.Offset} - {messagesRead.TopicPartitionOffset}");
                            }
                            catch (KafkaException e)
                            {
                                Console.WriteLine($"Commit error: {e.Error.Reason}");
                            }
                        }
                        catch (Exception ex)
                        {
                            //Logger.Error($"Kafka Consumer for topic: {_topic} - {ex.Message}");
                        }
                    }
                    //Logger.Error("Closing kafka consumer");
                }
            }
            catch (Exception ex)
            {
                //Logger.Error($"Kafka Consumer for topic: {_topic} - {ex.Message}");
            }

        }

        public void Dispose()
        {
        }
    }

}
