using Confluent.Kafka;
using Newtonsoft.Json;
using System.Text;

namespace PhoenixSagas.Kafka.Implementations
{
    public class JsonSerializer<T> : ISerializer<T>
    {
        public byte[] Serialize(T data, SerializationContext context)
        {
            var str = JsonConvert.SerializeObject(data);
            var bytes = Encoding.UTF8.GetBytes(str);
            return bytes;
        }
        
        public void Dispose()
        { }
    }
}
