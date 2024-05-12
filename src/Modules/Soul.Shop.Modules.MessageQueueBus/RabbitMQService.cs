using MassTransit;
using Soul.Shop.Modules.MessageQueueBus.Abstractions;

namespace Soul.Shop.Modules.MessageQueueBus
{
    public class RabbitMQService(IBusControl busControl) : IMQService
    {
        public async Task Send<T>(string queue, T message) where T : class
        {
            var sendEndpoint = await busControl.GetSendEndpoint(new Uri($"queue:{queue}"));
            await sendEndpoint.Send(message);
        }
    }
}
