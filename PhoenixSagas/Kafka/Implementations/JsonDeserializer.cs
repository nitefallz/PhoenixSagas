using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace PhoenixSagas.Kafka.Implementations
{
    public class OutputDeserializer<T> : IDeserializer<T>
    {
        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            using MemoryStream ms = new MemoryStream(data.ToArray());
            var message = Encoding.UTF8.GetString(data.ToArray());
            return JsonConvert.DeserializeObject<T>(message);
        }
    }
}
