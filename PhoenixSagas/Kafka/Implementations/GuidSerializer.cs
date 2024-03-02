using Confluent.Kafka;
using System;

namespace PhoenixSagas.Kafka.Implementations
{
    public class GuidSerializer : ISerializer<Guid>
    {
        public byte[] Serialize(Guid data, SerializationContext context) => data.ToByteArray();
        public void Dispose()
        { }
    }
}
