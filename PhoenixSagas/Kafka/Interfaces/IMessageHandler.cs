using System.Threading;
using System.Threading.Tasks;


namespace PhoenixSagas.Kafka.Interfaces
{
    public interface IMessageHandler<T>
    {
        Task HandleMessageAsync(T message, CancellationToken cancellationToken);
    }
}