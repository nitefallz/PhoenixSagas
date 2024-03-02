using Confluent.Kafka;
using System;

namespace PhoenixSagas.Kafka.Implementations
{
    public class GuidDeserializer : IDeserializer<Guid> 
    {
        public Guid Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context) => new Guid(data);
        public void Dispose()
        { } 
    }
}
