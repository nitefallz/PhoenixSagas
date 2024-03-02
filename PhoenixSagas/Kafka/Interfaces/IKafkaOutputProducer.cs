using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Kafka.Interfaces
{
    public interface IKafkaOutputProducer
    {
        Task Produce(int socketId, string message);
    }
}
