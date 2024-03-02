using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace PhoenixSagas.Kafka.Interfaces
{
    public interface IKafkaProducer<T>
    {
        EventHandler<T> MessagesHandler { get; set; }
        Task Produce(Message<Guid,T> message);
    }
}