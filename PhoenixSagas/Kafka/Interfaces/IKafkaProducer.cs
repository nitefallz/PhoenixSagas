using Confluent.Kafka;
using System;
using System.Threading.Tasks;

namespace PhoenixSagas.Kafka.Interfaces
{
    public interface IKafkaProducer<T>
    {
        Task Produce(Message<Guid,T> message);
    }
}